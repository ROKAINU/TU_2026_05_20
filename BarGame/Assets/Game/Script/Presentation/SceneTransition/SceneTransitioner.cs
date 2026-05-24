#nullable enable
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using VContainer;
using Game.Domain;
using Game.Infrastructure;
using Game.Presentation;
using Game.Presentation.View;
using Game.Kernel;
using Game.Kernel.Utils.Abstract;

namespace Game.Presentation.SceneTransition
{
    /// シーン遷移を管理するクラス
    public class SceneTransitioner : MonoBehaviour
    {
        /// <summary>
        /// シーン遷移を管理するクラス。
        /// ロード戦略は ISceneLoaderStrategy に委譲するため、
        /// Addressables / Scenes in Build を DI で差し替え可能。
        /// </summary>
        private ScreenFader _screenFader = null!;
        private BGMPlayer _bgmPlayer = null!;
        private SceneCatalog _sceneCatalog = null!;
        private LoggerBase _logger = null!;
        private ISceneLoaderStrategy _loaderStrategy = null!;

        [Inject]
        public void Construct(
            ScreenFader screenFader,
            BGMPlayer bgmPlayer,
            SceneCatalog sceneCatalog,
            LoggerBase logger,
            ISceneLoaderStrategy loaderStrategy)
        {
            _screenFader = screenFader;
            _bgmPlayer = bgmPlayer;
            _sceneCatalog = sceneCatalog;
            _logger = logger;
            _loaderStrategy = loaderStrategy;
        }

        // State
        private bool _isTransitioning;
        public bool IsTransitioning => _isTransitioning;

        public SceneInstance? LastLoadedScene { get; private set; }

        public event Action<float>? OnLoadProgress;
        public enum TransitionStatus { Success, Canceled, Failed, Rejected }

        public readonly struct TransitionResult
        {
            public TransitionStatus Status { get; }
            public SceneInstance? Scene { get; }
            public Exception? Error { get; }

            public TransitionResult(TransitionStatus status, SceneInstance? scene = null, Exception? error = null)
            {
                Status = status;
                Scene = scene;
                Error = error;
            }

            public static TransitionResult Success(SceneInstance scene) => new(TransitionStatus.Success, scene, null);
            public static TransitionResult Canceled(Exception? ex = null) => new(TransitionStatus.Canceled, null, ex);
            public static TransitionResult Failed(Exception ex) => new(TransitionStatus.Failed, null, ex);
            public static TransitionResult Rejected() => new(TransitionStatus.Rejected, null, null);
        }

        [Serializable]
        public struct TransitionOptions
        {
            public bool FadeOut;
            public bool FadeIn;
            public float FadeDuration;
            public bool UseLoadingOverlay;
            public bool CleanupOnFailure;
            public bool FadeOutBGM;
            public bool FadeInBGM;

            public TransitionOptions(
                bool fadeOut = true,
                bool fadeIn = true,
                float fadeDuration = 1f,
                bool useLoadingOverlay = true,
                bool cleanupOnFailure = true,
                bool fadeOutBGM = true,
                bool fadeInBGM = true)
            {
                FadeOut = fadeOut;
                FadeIn = fadeIn;
                FadeDuration = fadeDuration;
                UseLoadingOverlay = useLoadingOverlay;
                CleanupOnFailure = cleanupOnFailure;
                FadeOutBGM = fadeOutBGM;
                FadeInBGM = fadeInBGM;
            }
        }

        // -----------------------------------------------------------------------
        // プリロード API
        // -----------------------------------------------------------------------

        /// <summary>
        /// 指定シーンを事前にメモリへロードしておく。
        /// 遷移前の余裕があるタイミング（例：タイトル画面の表示中）に呼ぶ。
        /// 進捗は OnLoadProgress イベントで受け取れる。
        /// </summary>
        /// <param name="sceneId">事前ロードするシーン ID</param>
        /// <param name="ct">キャンセルトークン</param>
        public async UniTask PreloadSceneAsync(SceneId sceneId, CancellationToken ct = default)
        {
            if (!_loaderStrategy.SupportsPreload)
            {
                _logger.LogInfo("[SceneTransitioner] Current strategy does not support preload. Skipping.");
                return;
            }

            string address = GetAddressOrThrow(sceneId, "preload");
            var progress = new Progress<float>(v => OnLoadProgress?.Invoke(v));
            await _loaderStrategy.PreloadAsync(address, progress, ct);
        }

        // -----------------------------------------------------------------------
        // 遷移 API
        // -----------------------------------------------------------------------

        public async UniTask<TransitionResult> TransitionToAsync(
            SceneId sceneId,
            TransitionOptions? options = null,
            CancellationToken destroyCancellationToken = default)
        {
            if (_isTransitioning)
            {
                _logger.LogWarning("A scene transition is already in progress. Rejecting new transition request.");
                return TransitionResult.Rejected();
            }

            var opt = options ?? new TransitionOptions();
            string address = GetAddressOrThrow(sceneId, "transition");

            _isTransitioning = true;

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            var ct = linkedCts.Token;

            var snapshot = _bgmPlayer.CaptureSnapshot();

            try
            {
                // フェードアウト
                if (opt.FadeOut)
                {
                    if (opt.FadeOutBGM)
                    {
                        await UniTask.WhenAll(
                            _screenFader.FadeOut(opt.FadeDuration),
                            _bgmPlayer.TweenVolume(opt.FadeDuration, 0f)
                        );
                        _bgmPlayer.Stop();
                    }
                    else
                    {
                        await _screenFader.FadeOut(opt.FadeDuration);
                    }
                }
                
                // ロード（戦略に委譲）
                var progress = opt.UseLoadingOverlay
                    ? new Progress<float>(v => OnLoadProgress?.Invoke(v))
                    : null;

                SceneInstance loadedScene;

                if (_loaderStrategy.SupportsPreload && _loaderStrategy.IsCached(address))
                {
                    _logger.LogInfo($"[SceneTransitioner] Activating preloaded scene: {address}");
                    loadedScene = await _loaderStrategy.ActivateCachedAsync(address, progress, ct);
                }
                else
                {
                    _logger.LogInfo($"[SceneTransitioner] On-demand loading scene: {address}");
                    loadedScene = await _loaderStrategy.LoadOnDemandAsync(address, progress, ct);
                }

                LastLoadedScene = loadedScene;

                // フェードイン
                if (opt.FadeIn)
                {
                    if (opt.FadeInBGM)
                    {
                        // Load中にExecuter等がBGMのセットと再生を行うため、その際にこの音量変化が効果がある
                        _bgmPlayer.VolumeChange(0f);
                        
                        await UniTask.WhenAll(
                            _screenFader.FadeIn(opt.FadeDuration),
                            _bgmPlayer.TweenVolume(opt.FadeDuration, snapshot.Volume)
                        );
                    }
                    else
                    {
                        await _screenFader.FadeIn(opt.FadeDuration);
                    }
                }
                else
                    _screenFader.SetImmediateAlpha(0f);

                return TransitionResult.Success(loadedScene);
            }
            catch (OperationCanceledException oce)
            {
                _logger.LogInfo($"Transition canceled: {oce.Message}"); 
                
                if (opt.CleanupOnFailure)
                    await SafeCleanupOnFailureAsync(opt, snapshot);

                return TransitionResult.Canceled(oce);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "An error occurred during scene transition.");

                if (opt.CleanupOnFailure)
                    await SafeCleanupOnFailureAsync(opt, snapshot);

                return TransitionResult.Failed(ex);
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        // -----------------------------------------------------------------------
        // アンロード
        // -----------------------------------------------------------------------

        /// <summary>直前にロードしたシーンをアンロードする。</summary>
        public async UniTask<bool> UnloadLastLoadedSceneAsync(CancellationToken cancellationToken = default)
        {
            if (LastLoadedScene is null)
                return false;

            var scene = LastLoadedScene.Value;
            LastLoadedScene = null;

            await _loaderStrategy.UnloadAsync(scene, cancellationToken);
            return true;
        }

        // -----------------------------------------------------------------------
        // ヘルパー
        // -----------------------------------------------------------------------

        private string GetAddressOrThrow(SceneId sceneId, string context)
        {
            try
            {
                return _sceneCatalog.GetAddressOrThrow(sceneId);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"An error occurred while retrieving the scene address for {context}.");
                throw;
            }
        }

        private async UniTask SafeCleanupOnFailureAsync(TransitionOptions opt, BGMSnapshot snapshot)
        {
            try
            {
                if (opt.FadeOut)
                {
                    if (opt.FadeOutBGM && snapshot.Clip != null)
                    {
                        _bgmPlayer.PlayClipRaw(snapshot.Clip, 0f);

                        await UniTask.WhenAll(
                            _screenFader.FadeIn(opt.FadeDuration),
                            _bgmPlayer.TweenVolume(opt.FadeDuration, snapshot.Volume)
                        );
                    }
                    else
                    {
                        await _screenFader.FadeIn(opt.FadeDuration);
                    }
                }
                else
                    _screenFader.SetImmediateAlpha(0f);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "An error occurred while cleaning up after a failed transition.");
                // フェードの失敗は致命的ではないため、例外を再スローしない
            }
        }

        // OnDestroy は不要：IAssetPreloader.Dispose() が LifetimeScope 経由で呼ばれるため
    }
}