#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;


namespace Game.Kernel.Utils.Abstruct
{
    /// <summary>
    /// アセットの事前ロード（プリロード）と取得を担うインターフェース。
    /// </summary>
    public interface IAssetPreloader : IDisposable
    {
        // -----------------------------------------------------------------------
        // アセット（UnityEngine.Object）用
        // -----------------------------------------------------------------------
 
        /// <summary>
        /// 指定アドレスのアセットをメモリにロードして保持する。
        /// 既にキャッシュ済みの場合は何もしない。
        /// </summary>
        /// <typeparam name="T">ロードするアセットの型</typeparam>
        /// <param name="address">Addressables アドレス</param>
        /// <param name="progress">進捗コールバック（0.0〜1.0）。不要な場合は null</param>
        /// <param name="ct">キャンセルトークン</param>
        UniTask PreloadAsync<T>(
            string address,
            IProgress<float>? progress = null,
            CancellationToken ct = default) where T : UnityEngine.Object;
 
        /// <summary>
        /// 複数アドレスをまとめてプリロードする。
        /// progress は全アドレスの平均進捗を報告する。
        /// </summary>
        /// <typeparam name="T">ロードするアセットの型</typeparam>
        /// <param name="addresses">Addressables アドレスの列挙</param>
        /// <param name="progress">進捗コールバック（0.0〜1.0）。不要な場合は null</param>
        /// <param name="ct">キャンセルトークン</param>
        UniTask PreloadAllAsync<T>(
            IEnumerable<string> addresses,
            IProgress<float>? progress = null,
            CancellationToken ct = default) where T : UnityEngine.Object;
 
        /// <summary>
        /// キャッシュ済みアセットを即座に返す。
        /// PreloadAsync 完了前に呼ぶと InvalidOperationException をスローする。
        /// </summary>
        T Get<T>(string address) where T : UnityEngine.Object;
 
        /// <summary>指定アドレスのアセットがキャッシュ済みかどうかを返す。</summary>
        bool IsCached(string address);
 
        /// <summary>指定アドレスのアセットキャッシュを解放する。</summary>
        void Release(string address);
 
        // -----------------------------------------------------------------------
        // シーン用
        // SceneInstance は UnityEngine.Object を継承しないため専用メソッドで管理する。
        // activateOnLoad: false でロードし、遷移タイミングで ActivateAsync() を呼ぶ設計。
        //
        // 【解放タイミングの目安】
        //   - TransitionToAsync() 内でシーン有効化後に自動解放される。
        //   - 遷移をキャンセルした場合は ReleaseScene() を明示的に呼ぶこと。
        //   - Dispose() で全シーンキャッシュをまとめて解放できる。
        // -----------------------------------------------------------------------
 
        /// <summary>
        /// 指定アドレスのシーンを activateOnLoad:false でプリロードする。
        /// 既にキャッシュ済みの場合は何もしない。
        /// </summary>
        /// <param name="address">Addressables シーンアドレス</param>
        /// <param name="progress">進捗コールバック（0.0〜1.0）。不要な場合は null</param>
        /// <param name="ct">キャンセルトークン</param>
        UniTask PreloadSceneAsync(
            string address,
            IProgress<float>? progress = null,
            CancellationToken ct = default);
 
        /// <summary>プリロード済みシーンを取得する。未プリロードの場合は null を返す。</summary>
        SceneInstance? GetPreloadedScene(string address);
 
        /// <summary>指定アドレスのシーンがキャッシュ済みかどうかを返す。</summary>
        bool IsSceneCached(string address);
 
        /// <summary>
        /// 指定アドレスのシーンキャッシュを解放する。
        /// TransitionToAsync() 内で自動解放されるが、遷移キャンセル時などに手動で呼ぶ。
        /// </summary>
        void ReleaseScene(string address);
    }

}