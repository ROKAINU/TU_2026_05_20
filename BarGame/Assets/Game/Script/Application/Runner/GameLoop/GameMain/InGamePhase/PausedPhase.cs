using System;
using System.Threading;
using System.Threading.Tasks;
using Game.Domain;
using Game.Application.Contracts;
using Game.Kernel;
using Game.Kernel.Utils.Cysharp;

namespace Game.Application.Runner
{
    /// Paused フェーズ
    public class PausedPhase : IInGamePhase
    {
        private readonly AsyncCommandQueue<GameCommand> _input;
        private readonly IEventPublisher<GamePauseMessage> _gamePausePublisher;
        private Action<GameState.State> _onRequestStateChange;

        public PausedPhase(
            AsyncCommandQueue<GameCommand> input,
            IEventPublisher<GamePauseMessage> gamePausePublisher)
        {
            _input = input;
            _gamePausePublisher = gamePausePublisher;
        }

        public void SetStateChangeCallback(Action<GameState.State> callback)
        {
            _onRequestStateChange = callback;
        }

        public async Task<GameMainLoopResult?> TickAsync(float dt, CancellationToken ct)
        {
            while (_input.TryDequeue(out var cmd))
            {
                switch (cmd)
                {
                    case GameCommand.Resume:
                    case GameCommand.Pause:
                        _onRequestStateChange?.Invoke(GameState.State.Playing);
                        await _gamePausePublisher.PublishAsync(new GamePauseMessage(isPausing: false), ct);
                        break;
                        
                    default:
                        break;
                }
            }

            return null; // 継続中
        }
    }
}