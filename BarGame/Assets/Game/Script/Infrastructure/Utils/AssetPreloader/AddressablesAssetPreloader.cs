#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Game.Kernel;
using Game.Kernel.Utils.Abstract;

namespace Game.Infrastructure.AssetPreloader
{
    /// <summary>
    /// Addressables を使った IAssetPreloader の実装。
    /// VContainer で IAssetPreloader にバインドして使う。
    ///
    /// 登録例（LifetimeScope）:
    ///   builder.Register&lt;AddressablesAssetPreloader&gt;(Lifetime.Singleton).As&lt;IAssetPreloader&gt;();
    /// </summary>
    public sealed class AddressablesAssetPreloader : IAssetPreloader
    {
        // アセット用キャッシュ（address → 型消去ハンドル）
        private readonly Dictionary<string, AsyncOperationHandle> _assetCache = new();

        // シーン用キャッシュ（SceneInstance は UnityEngine.Object 非継承のため分離）
        private readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> _sceneCache = new();

        private readonly LoggerBase _logger;
        private bool _disposed;

        public AddressablesAssetPreloader(LoggerBase logger)
        {
            _logger = logger;
        }

        // -----------------------------------------------------------------------
        // アセット（UnityEngine.Object）用
        // -----------------------------------------------------------------------

        /// <inheritdoc/>
        public async UniTask PreloadAsync<T>(
            string address,
            IProgress<float>? progress = null,
            CancellationToken ct = default) where T : UnityEngine.Object
        {
            ThrowIfDisposed();

            if (_assetCache.ContainsKey(address))
            {
                progress?.Report(1f);
                return;
            }

            _logger.LogInfo($"[AssetPreloader] Start preload: {address}");

            var handle = Addressables.LoadAssetAsync<T>(address);

            try
            {
                // 進捗をポーリングしながら待機
                while (!handle.IsDone)
                {
                    ct.ThrowIfCancellationRequested();
                    progress?.Report(Mathf.Clamp01(handle.PercentComplete));
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }

                if (handle.Status != AsyncOperationStatus.Succeeded)
                    throw handle.OperationException ?? new Exception($"LoadAssetAsync failed: {address}");
            }
            catch (OperationCanceledException)
            {
                Addressables.Release(handle);
                _logger.LogWarning($"[AssetPreloader] Preload canceled: {address}");
                throw;
            }
            catch (Exception ex)
            {
                Addressables.Release(handle);
                _logger.LogException(ex, $"[AssetPreloader] Failed to preload: {address}");
                throw;
            }

            _assetCache[address] = handle;
            progress?.Report(1f);
            _logger.LogInfo($"[AssetPreloader] Preload complete: {address}");
        }

        /// <inheritdoc/>
        public async UniTask PreloadAllAsync<T>(
            IEnumerable<string> addresses,
            IProgress<float>? progress = null,
            CancellationToken ct = default) where T : UnityEngine.Object
        {
            ThrowIfDisposed();

            var addressList = new List<string>(addresses);
            if (addressList.Count == 0) return;

            // 各アドレスの進捗を配列で持ち、平均を progress に流す
            var perProgress = new float[addressList.Count];
            var tasks = new List<UniTask>(addressList.Count);

            for (int i = 0; i < addressList.Count; i++)
            {
                int idx = i; // クロージャ用
                var individualProgress = progress == null
                    ? null
                    : new Progress<float>(v =>
                    {
                        perProgress[idx] = v;
                        float avg = 0f;
                        foreach (var p in perProgress) avg += p;
                        progress.Report(avg / perProgress.Length);
                    });

                tasks.Add(PreloadAsync<T>(addressList[i], individualProgress, ct));
            }

            await UniTask.WhenAll(tasks);
        }

        /// <inheritdoc/>
        public T Get<T>(string address) where T : UnityEngine.Object
        {
            ThrowIfDisposed();

            if (!_assetCache.TryGetValue(address, out var handle))
                throw new InvalidOperationException(
                    $"[AssetPreloader] Asset not preloaded: '{address}'. Call PreloadAsync first.");

            if (handle.Result is not T result)
                throw new InvalidCastException(
                    $"[AssetPreloader] Cached asset '{address}' is not of type {typeof(T).Name}.");

            return result;
        }

        /// <inheritdoc/>
        public bool IsCached(string address)
        {
            ThrowIfDisposed();
            return _assetCache.ContainsKey(address);
        }

        /// <inheritdoc/>
        public void Release(string address)
        {
            ThrowIfDisposed();

            if (!_assetCache.TryGetValue(address, out var handle)) return;

            Addressables.Release(handle);
            _assetCache.Remove(address);
            _logger.LogInfo($"[AssetPreloader] Released asset: {address}");
        }

        // -----------------------------------------------------------------------
        // シーン用
        // -----------------------------------------------------------------------

        /// <inheritdoc/>
        public async UniTask PreloadSceneAsync(
            string address,
            IProgress<float>? progress = null,
            CancellationToken ct = default)
        {
            ThrowIfDisposed();

            if (_sceneCache.ContainsKey(address))
            {
                progress?.Report(1f);
                _logger.LogInfo($"[AssetPreloader] Scene already preloaded: {address}");
                return;
            }

            _logger.LogInfo($"[AssetPreloader] Start scene preload: {address}");

            // activateOnLoad: false でロード → 遷移タイミングで ActivateAsync() する
            var handle = Addressables.LoadSceneAsync(address, activateOnLoad: false);

            try
            {
                while (!handle.IsDone)
                {
                    ct.ThrowIfCancellationRequested();
                    progress?.Report(Mathf.Clamp01(handle.PercentComplete));
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }

                if (handle.Status != AsyncOperationStatus.Succeeded)
                    throw handle.OperationException ?? new Exception($"LoadSceneAsync failed: {address}");
            }
            catch (OperationCanceledException)
            {
                Addressables.Release(handle);
                _logger.LogWarning($"[AssetPreloader] Scene preload canceled: {address}");
                throw;
            }
            catch (Exception ex)
            {
                Addressables.Release(handle);
                _logger.LogException(ex, $"[AssetPreloader] Scene preload failed: {address}");
                throw;
            }

            _sceneCache[address] = handle;
            progress?.Report(1f);
            _logger.LogInfo($"[AssetPreloader] Scene preload complete: {address}");
        }

        /// <inheritdoc/>
        public SceneInstance? GetPreloadedScene(string address)
        {
            ThrowIfDisposed();

            if (!_sceneCache.TryGetValue(address, out var handle)) return null;
            return handle.Result;
        }

        /// <inheritdoc/>
        public bool IsSceneCached(string address)
        {
            ThrowIfDisposed();
            return _sceneCache.ContainsKey(address);
        }

        /// <inheritdoc/>
        public void ReleaseScene(string address)
        {
            ThrowIfDisposed();

            if (!_sceneCache.TryGetValue(address, out var handle)) return;

            Addressables.Release(handle);
            _sceneCache.Remove(address);
            _logger.LogInfo($"[AssetPreloader] Released scene: {address}");
        }

        // -----------------------------------------------------------------------
        // IDisposable
        // -----------------------------------------------------------------------

        /// <summary>全キャッシュ（アセット・シーン）を解放する。</summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            foreach (var handle in _assetCache.Values)
                Addressables.Release(handle);
            _assetCache.Clear();

            foreach (var handle in _sceneCache.Values)
                Addressables.Release(handle);
            _sceneCache.Clear();

            _logger.LogInfo("[AssetPreloader] Disposed. All cached assets and scenes released.");
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AddressablesAssetPreloader));
        }
    }
}