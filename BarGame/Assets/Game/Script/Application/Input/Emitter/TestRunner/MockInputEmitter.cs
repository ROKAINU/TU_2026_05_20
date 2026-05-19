#nullable enable
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Kernel.Utils.Cysharp;
using UnityEngine;

namespace Game.Application
{
    /// <summary>
    /// テスト用の自動化入力を提供するEmitter
    /// 事前に定義されたInputSequenceを再生
    /// </summary>
    public sealed class MockInputEmitter : ICommandEmitter
    {
        private readonly IAsyncCommandQueue<GameCommand> _queue;
        private IInputSequence? _currentSequence;
        private CancellationTokenSource? _cts;
        private bool _enabled;
        
        /// <summary>現在実行中のシーケンス名</summary>
        public string? CurrentSequenceName => _currentSequence?.Name;
        
        /// <summary>シーケンスが完了したかどうか</summary>
        public bool IsSequenceCompleted => _currentSequence?.IsCompleted ?? true;

        public MockInputEmitter(IAsyncCommandQueue<GameCommand> queue)
        {
            _queue = queue;
        }

        /// <summary>
        /// テスト用のInputSequenceを設定して開始
        /// </summary>
        public void PlaySequence(IInputSequence sequence)
        {
            _currentSequence = sequence ?? throw new ArgumentNullException(nameof(sequence));
            _currentSequence.Reset();
            
            if (!_enabled)
            {
                Enable();
            }
        }

        /// <summary>
        /// 複数のシーケンスを順番に実行
        /// </summary>
        public async UniTask PlaySequencesAsync(IInputSequence[] sequences, CancellationToken externalToken = default)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            
            foreach (var sequence in sequences)
            {
                PlaySequence(sequence);
                
                // シーケンスが完了するまで待機
                while (!sequence.IsCompleted && !cts.Token.IsCancellationRequested)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, cts.Token);
                }
                
                await UniTask.Delay(100, cancellationToken: cts.Token); // シーケンス間の小休止
            }
        }

        public void Enable()
        {
            if (_enabled) return;
            _enabled = true;
            _cts = new CancellationTokenSource();
            PumpAsync(_cts.Token).Forget();
        }

        public void Disable()
        {
            if (!_enabled) return;
            _enabled = false;
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        public void Dispose() => Disable();

        private async UniTaskVoid PumpAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, ct);

                if (_currentSequence != null && !_currentSequence.IsCompleted)
                {
                    var (action, isReady) = _currentSequence.GetNextAction(Time.deltaTime);
                    
                    if (isReady && action.HasValue)
                    {
                        // コマンドキューにエンキュー
                        _queue.Enqueue(new GameCommand(action.Value.CommandType));
                    }
                }
            }
        }
    }
}