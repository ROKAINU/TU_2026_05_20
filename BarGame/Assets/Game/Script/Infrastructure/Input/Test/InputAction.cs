#nullable enable
using System;
using System.Collections.Generic;
using Game.Domain;

namespace Game.Infrastructure
{
    /// <summary>
    /// テストシナリオで再生する入力アクション
    /// </summary>
    public readonly struct InputAction
    {
        /// <summary>実行するコマンドタイプ</summary>
        public GameCommand Command { get; }
        
        /// <summary>フレームから次のアクションまでの遅延（秒）</summary>
        public float DelaySeconds { get; }
        
        /// <summary>繰り返し回数（1がデフォルト）</summary>
        public int RepeatCount { get; }
        
        public InputAction(GameCommand command, float delaySeconds = 0.1f, int repeatCount = 1)
        {
            if (delaySeconds < 0)
                throw new ArgumentException("DelaySeconds must be non-negative.", nameof(delaySeconds));
            if (repeatCount <= 0)
                throw new ArgumentException("RepeatCount must be greater than 0.", nameof(repeatCount));

            Command      = command;
            DelaySeconds = delaySeconds;
            RepeatCount  = repeatCount;
        }
    }
}