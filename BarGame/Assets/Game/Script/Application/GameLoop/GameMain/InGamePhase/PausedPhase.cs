using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Game.Domain;
using Game.Application.Contracts;
using Game.Kernel.Utils.Cysharp;

namespace Game.Application
{
    /// Paused フェーズ
    public class PausedPhase : IInGamePhase
    {
        private readonly IAsyncCommandQueue<GameCommand> _input;
        private readonly IAsyncPublisher<GamePauseMessage> _gamePausePublisher;
        private Action<GameState.State> _onRequestStateChange;

        public PausedPhase(
            IAsyncCommandQueue<GameCommand> input,
            IAsyncPublisher<GamePauseMessage> gamePausePublisher)
        {
            _input = input;
            _gamePausePublisher = gamePausePublisher;
        }

        public void SetStateChangeCallback(Action<GameState.State> callback)
        {
            _onRequestStateChange = callback;
        }

        public async UniTask<GameMainLoopResult?> TickAsync(float dt, CancellationToken ct)
        {
            while (_input.TryDequeue(out var cmd))
            {
                switch (cmd.Type)
                {
                    case GameCommandType.Resume:
                    case GameCommandType.Pause:
                        _onRequestStateChange?.Invoke(GameState.State.Playing);
                        await _gamePausePublisher.PublishAsync(new GamePauseMessage(false), ct);
                        break;
                        
                    default:
                        break;
                }
            }

            return null; // 継続中
        }
    }
}