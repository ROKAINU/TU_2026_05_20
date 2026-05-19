#nullable enable

using Game.Kernel;
using UnityEngine;

namespace Game.Infrastructure
{
    /// <summary>
    /// Unity Time 実装（scaled）。
    /// </summary>
    public sealed class UnityGameTime : IGameTime
    {
        public float DeltaTime => Time.deltaTime;
        public float Now => Time.time;
        public float RealtimeSinceStartup => Time.realtimeSinceStartup;
    }

    /// <summary>
    /// Unity Time 実装（unscaled）。
    /// </summary>
    public sealed class UnityUnscaledTime : IUnscaledTime
    {
        public float DeltaTime => Time.unscaledDeltaTime;
        public float Now => Time.unscaledTime;
        public float RealtimeSinceStartup => Time.realtimeSinceStartup;
    }

    /// <summary>
    /// Unity Time 実装（fixed）。
    /// 注意: FixedUpdate 以外から参照すると期待どおりでない場合があります。
    /// </summary>
    public sealed class UnityFixedTime : IFixedTime
    {
        public float DeltaTime => Time.fixedDeltaTime;
        public float Now => Time.fixedTime;
        public float RealtimeSinceStartup => Time.realtimeSinceStartup;
    }
}