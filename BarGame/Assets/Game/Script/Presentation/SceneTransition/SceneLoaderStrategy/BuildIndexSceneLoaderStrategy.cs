#nullable enable
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Game.Presentation.SceneTransition
{
    /// <summary>
    /// Scenes in Build（BuildIndex）を使ったシーンロード戦略。
    /// Addressables 非対応環境（UnityRoom など）向け。
    /// プリロードは非サポート（SupportsPreload = false）。
    /// </summary>
    public sealed class BuildIndexSceneLoaderStrategy : ISceneLoaderStrategy
    {
        // -----------------------------------------------------------------------
        // プリロード（非サポート）
        // -----------------------------------------------------------------------

        public bool SupportsPreload => false;

        // SupportsPreload = false のため SceneTransitioner からは呼ばれないが、
        // インターフェース上の契約として明示的に実装しておく。
        public UniTask PreloadAsync(string address, IProgress<float>? progress, CancellationToken ct)
        {
            Debug.LogWarning("[BuildIndexSceneLoaderStrategy] PreloadAsync is not supported.");
            return UniTask.CompletedTask;
        }

        public bool IsCached(string address) => false;

        public UniTask<SceneInstance> ActivateCachedAsync(string address, IProgress<float>? progress, CancellationToken ct)
            => throw new NotSupportedException("BuildIndexSceneLoaderStrategy does not support preload/cache.");

        // -----------------------------------------------------------------------
        // オンデマンドロード
        // -----------------------------------------------------------------------

        public async UniTask<SceneInstance> LoadOnDemandAsync(
            string address,
            IProgress<float>? progress,
            CancellationToken ct)
        {
            // address をシーン名として SceneManager でロード
            // BuildSettings に登録されているシーン名と一致させること
            var op = SceneManager.LoadSceneAsync(address, LoadSceneMode.Single)
                     ?? throw new Exception($"SceneManager.LoadSceneAsync returned null for address: {address}");

            op.allowSceneActivation = true;

            while (!op.isDone)
            {
                ct.ThrowIfCancellationRequested();
                progress?.Report(Mathf.Clamp01(op.progress / 0.9f)); // 0〜1 に正規化
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            progress?.Report(1f);

            // BuildIndex 方式には SceneInstance の概念がないため
            // 空の SceneInstance を返す（SceneTransitioner 側で LastLoadedScene として保持される）
            return default;
        }

        // -----------------------------------------------------------------------
        // アンロード
        // -----------------------------------------------------------------------

        public async UniTask UnloadAsync(SceneInstance scene, CancellationToken ct)
        {
            // BuildIndex 方式では SceneInstance は空なので、
            // アンロードが必要な場合は呼び出し元で Scene 参照を別途管理すること。
            // ここでは何もしない（UnloadLastLoadedScene は実質 no-op）。
            await UniTask.CompletedTask;
            Debug.LogWarning("[BuildIndexSceneLoaderStrategy] UnloadAsync is a no-op in BuildIndex mode. " +
                             "Use SceneManager.UnloadSceneAsync() directly if needed.");
        }
    }
}