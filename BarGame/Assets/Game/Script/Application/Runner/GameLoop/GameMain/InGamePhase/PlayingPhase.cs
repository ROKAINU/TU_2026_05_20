using System;
using System.Threading;
using System.Threading.Tasks;
using Game.Domain;
using Game.Application;
using Game.Application.Contracts;
using Game.Kernel;
using Game.Kernel.Utils.Cysharp;

namespace Game.Application.Runner
{
    /// Playing フェーズ
    public class PlayingPhase : IInGamePhase
    {
        private readonly IStore<GameMainState> _gameMainStore;
        private readonly IStore<GameGlobalState> _globalStore;
        private readonly AsyncCommandQueue<GameCommand> _input;
        private readonly ICommandHandler<GameCommand> _handler;
        private readonly IEventPublisher<GamePauseMessage> _gamePausePublisher;
        
        // 状態遷移コールバック
        private Action<GameState.State> _onRequestStateChange;

        public PlayingPhase(
            IStore<GameMainState> gameMainStore,
            IStore<GameGlobalState> globalStore,
            AsyncCommandQueue<GameCommand> input,
            ICommandHandler<GameCommand> handler,
            IEventPublisher<GamePauseMessage> gamePausePublisher)
        {
            _gameMainStore = gameMainStore;
            _globalStore = globalStore;
            _input = input;
            _handler = handler;
            _gamePausePublisher = gamePausePublisher;
        }

        public void SetStateChangeCallback(Action<GameState.State> callback)
        {
            _onRequestStateChange = callback;
        }

        public async Task<GameMainLoopResult?> TickAsync(float dt, CancellationToken ct)
        {
            // 時間減少
            _gameMainStore.Dispatch(GameMainStateReducer.DecreaseRemainingTime(dt));

            // 終了条件
            if (GameMainRule.IsTimeUp(_gameMainStore.CurrentState))
                return GameMainLoopResult.Cleared;

            // 入力読み取り
            while (_input.TryDequeue(out var cmd))
            {
                switch (cmd)
                {
                    case GameCommand.Pause:
                        _onRequestStateChange?.Invoke(GameState.State.Paused);
                        await _gamePausePublisher.PublishAsync(new GamePauseMessage(isPausing: true), ct);
                        break;
                        
                    case GameCommand.AddScore:
                        await _handler.HandleAsync(cmd, ct);
                        break;

                    case GameCommand.ShowJson:
                        await _handler.HandleAsync(cmd, ct);
                        break;
                        
                    default:
                        break;
                }
            }

            return null; // 継続中
        }
    }
}