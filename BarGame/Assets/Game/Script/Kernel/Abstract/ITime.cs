#nullable enable

namespace Game.Kernel
{
    /// <summary>
    /// 時間抽象。Game/Unscaled/Fixed など用途別の実装で時間スケール依存を分離する。
    /// </summary>
    public interface ITime
    {
        /// <summary>この "Time" の deltaTime（例: scaled / unscaled / fixed）。</summary>
        float DeltaTime { get; }

        /// <summary>この "Time" の経過時間（例: Time.time / Time.unscaledTime / fixedTime）。</summary>
        float Now { get; }

        /// <summary>実時間（スケール非依存）。Unity の Time.realtimeSinceStartup 相当。</summary>
        float RealtimeSinceStartup { get; }
    }

    /// <summary>スケール適用（Time.deltaTime）</summary>
    public interface IGameTime : ITime { }

    /// <summary>スケール非依存（Time.unscaledDeltaTime）</summary>
    public interface IUnscaledTime : ITime { }

    /// <summary>固定ステップ（Time.fixedDeltaTime）</summary>
    public interface IFixedTime : ITime { }
}