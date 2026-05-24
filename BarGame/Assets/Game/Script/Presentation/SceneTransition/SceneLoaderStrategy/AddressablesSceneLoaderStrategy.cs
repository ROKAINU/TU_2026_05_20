#nullable enable
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Game.Infrastructure;
using Game.Kernel.Utils.Abstract;

namespace Game.Presentation.SceneTransition
{
    /// <summary>
    /// Addressables を使ったシーンロード戦略。
    /// プリロード・オンデマンドロード・アンロードを IAssetPreloader 経由で行う。
    /// </summary>
    public sealed class AddressablesSceneLoaderStrategy : ISceneLoaderStrategy
    {
        private readonly IAssetPreloader _assetPreloader;

        public AddressablesSceneLoaderStrategy(IAssetPreloader assetPreloader)
        {
            _assetPreloader = assetPreloader;
        }

        // -----------------------------------------------------------------------
        // プリロード
        // -----------------------------------------------------------------------

        public bool SupportsPreload => true;

        public UniTask PreloadAsync(string address, IProgress<float>? progress, CancellationToken ct)
            => _assetPreloader.PreloadSceneAsync(address, progress, ct);

        public bool IsCached(string address)
            => _assetPreloader.IsSceneCached(address);

        // -----------------------------------------------------------------------
        // ロード / アクティベート
        // -----------------------------------------------------------------------

        public async UniTask<SceneInstance> ActivateCachedAsync(
            string address,
            IProgress<float>? progress,
            CancellationToken ct)
        {
            var scene = _assetPreloader.GetPreloadedScene(address)!.Value;
            await scene.ActivateAsync().ToUniTask(cancellationToken: ct);

            // アクティベート済みなのでキャッシュを解放
            _assetPreloader.ReleaseScene(address);

            progress?.Report(1f);
            return scene;
        }

        public async UniTask<SceneInstance> LoadOnDemandAsync(
            string address,
            IProgress<float>? progress,
            CancellationToken ct)
        {
            var handle = Addressables.LoadSceneAsync(address);
            return await AwaitWithProgress(handle, progress, ct);
        }

        // -----------------------------------------------------------------------
        // アンロード
        // -----------------------------------------------------------------------

        public async UniTask UnloadAsync(SceneInstance scene, CancellationToken ct)
        {
            var handle = Addressables.UnloadSceneAsync(scene);
            await handle.ToUniTask(cancellationToken: ct);
        }

        // -----------------------------------------------------------------------
        // ヘルパー：プログレス付きロード待機
        // -----------------------------------------------------------------------

        /// <summary>
        /// Addressables のシーンロードを待ちながら進捗をスムーズに報告する。
        /// Addressables は完了直前まで 0.9 を返すため Lerp で補間している。
        /// </summary>
        private static async UniTask<SceneInstance> AwaitWithProgress(
            AsyncOperationHandle<SceneInstance> handle,
            IProgress<float>? progress,
            CancellationToken ct)
        {
            const float smoothSpeed = 10f;
            float shown = 0f;

            while (!handle.IsDone)
            {
                ct.ThrowIfCancellationRequested();

                float raw    = Mathf.Clamp01(handle.PercentComplete);
                float target = Mathf.Min(raw, 0.9f);
                shown = Mathf.Lerp(shown, target, 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime));

                progress?.Report(shown);

                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            if (handle.Status != AsyncOperationStatus.Succeeded)
                throw handle.OperationException ?? new Exception("Addressables.LoadSceneAsync failed.");

            progress?.Report(1f);
            return handle.Result;
        }
    }
}