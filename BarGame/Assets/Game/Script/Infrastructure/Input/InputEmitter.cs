#nullable enable
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Domain;
using Game.Application.Contracts;
using Game.Kernel.Utils.Cysharp;

namespace Game.Infrastructure
{
    /// <summary>
    /// キーボード入力を検知してゲームコマンドを発行するEmitter
    /// </summary>
    public sealed class InputEmitter : ICommandEmitter
    {
        private readonly AsyncCommandQueue<GameCommand> _queue;
        private readonly IInputService _inputService;
        private CancellationTokenSource? _cts;
        private bool _enabled;

        public InputEmitter(AsyncCommandQueue<GameCommand> queue, IInputService inputService)
        {
            _queue = queue;
            _inputService = inputService;
        }
        
        /// <summary>
        /// Emitterを有効にする
        /// </summary>
        public void Enable()
        {
            if (_enabled) return;
            _enabled = true;

            _cts = new CancellationTokenSource();
            PumpAsync(_cts.Token).Forget();
        }

        /// <summary>
        /// Emitterを停止する。これにより、キーボード入力の検知も停止される。
        /// </summary>
        public void Disable()
        {
            if (!_enabled) return;
            _enabled = false;

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        public void Dispose() => Disable();

        /// <summary>
        /// キーボード入力を検知してゲームコマンドを発行する非同期ループ
        /// </summary>
        /// <param name="ct">キャンセル用のトークン</param>
        private async UniTaskVoid PumpAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.PreUpdate, ct);

                if (_inputService.IsPausePressed())
                    _queue.Enqueue(new GameCommand.Pause());

                if (_inputService.GetAddScoreInput())
                    _queue.Enqueue(new GameCommand.AddScore(1));

                if (_inputService.IsShowJsonPressed() is int jsonKey && jsonKey > 0 && jsonKey <= 3)
                    _queue.Enqueue(new GameCommand.ShowJson(jsonKey));
            }
        }
    }
}