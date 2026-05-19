using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using VContainer;
using MessagePipe;
using Game.Domain;
using Game.Application.Contracts;
using Game.Kernel.Utils.Cysharp;
using Game.Kernel;
using Game.Kernel.Utils.R3;

namespace Game.Application
{
    public sealed class GameMainLoop
    {
        private readonly PlayingPhase _playingPhase;
        private readonly PausedPhase _pausedPhase;

        private readonly IGameTime _time;
        
        private readonly GameState _currentState;
        private IInGamePhase _currentPhase;

        public GameMainLoop(
            PlayingPhase playingPhase,
            PausedPhase pausedPhase,
            IGameTime time)
        {
            _playingPhase = playingPhase;
            _pausedPhase = pausedPhase;
            _time = time;
            
            _currentState = new GameState(GameState.State.Playing);

            // 状態遷移時のコールバック
            _currentState.OnStateEntered += (state) =>
            {
                _currentPhase = state switch
                {
                    GameState.State.Playing => _playingPhase,
                    GameState.State.Paused => _pausedPhase,
                    _ => _playingPhase,
                };
            };
            
            // 各フェーズに状態遷移要求を渡す
            _playingPhase.SetStateChangeCallback(newState => _currentState.ChangeState(newState));
            _pausedPhase.SetStateChangeCallback(newState => _currentState.ChangeState(newState));
            
            _currentPhase = _playingPhase;
        }

        public async UniTask<GameMainLoopResult> RunAsync(CancellationToken ct)
        {
            while (true)
            {
                if (ct.IsCancellationRequested) return GameMainLoopResult.Canceled;

                await UniTask.Yield(PlayerLoopTiming.Update, ct);

                // 現在のフェーズを実行
                var result = await _currentPhase.TickAsync(_time.DeltaTime, ct);
                
                if (result.HasValue)
                    return result.Value;
            }
        }
    }
}