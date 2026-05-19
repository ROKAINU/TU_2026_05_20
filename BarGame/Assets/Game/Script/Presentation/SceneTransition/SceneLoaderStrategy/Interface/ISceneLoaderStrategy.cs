#nullable enable
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Game.Presentation.SceneTransition
{
    /// <summary>
    /// シーンのロード戦略を抽象化するインターフェース。
    /// Addressables / Scenes in Build など、複数の実装を差し替え可能にする。
    /// </summary>
    public interface ISceneLoaderStrategy
    {
        // -----------------------------------------------------------------------
        // プリロード
        // -----------------------------------------------------------------------

        /// <summary>
        /// この戦略がプリロードをサポートするか。
        /// false の場合、SceneTransitioner はプリロードを無視する。
        /// </summary>
        bool SupportsPreload { get; }

        /// <summary>
        /// 指定アドレスのシーンを事前にメモリへロードする。
        /// SupportsPreload が false の場合は呼ばれない。
        /// </summary>
        UniTask PreloadAsync(string address, IProgress<float>? progress, CancellationToken ct);

        /// <summary>プリロード済みシーンがキャッシュされているか。</summary>
        bool IsCached(string address);

        // -----------------------------------------------------------------------
        // ロード / アクティベート
        // -----------------------------------------------------------------------

        /// <summary>
        /// キャッシュ済みシーンをアクティベートする。
        /// IsCached が true のときのみ SceneTransitioner から呼ばれる。
        /// </summary>
        UniTask<SceneInstance> ActivateCachedAsync(string address, IProgress<float>? progress, CancellationToken ct);

        /// <summary>
        /// オンデマンドでシーンをロードする。
        /// IsCached が false のとき SceneTransitioner から呼ばれる。
        /// </summary>
        UniTask<SceneInstance> LoadOnDemandAsync(string address, IProgress<float>? progress, CancellationToken ct);

        // -----------------------------------------------------------------------
        // アンロード
        // -----------------------------------------------------------------------

        /// <summary>ロード済みシーンをアンロードする。</summary>
        UniTask UnloadAsync(SceneInstance scene, CancellationToken ct);
    }
}