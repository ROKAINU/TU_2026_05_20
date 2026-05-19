using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Game.Domain;
using Game.Application;
using Game.Application.Contracts;
using Game.Kernel;
using Game.Kernel.Utils.Cysharp;
using Game.Kernel.Utils.R3;

namespace Game.Application
{
    /// Playing フェーズ
    public class PlayingPhase : IInGamePhase
    {
        private readonly Store<GameMainState> _gameMainStore;
        private readonly Store<GameGlobalState> _globalStore;
        private readonly IAsyncCommandQueue<GameCommand> _input;
        private readonly ICommandHandler<GameCommand> _handler;
        private readonly IAsyncPublisher<GamePauseMessage> _gamePausePublisher;
        
        // 状態遷移コールバック
        private Action<GameState.State> _onRequestStateChange;

        public PlayingPhase(
            Store<GameMainState> gameMainStore,
            Store<GameGlobalState> globalStore,
            IAsyncCommandQueue<GameCommand> input,
            ICommandHandler<GameCommand> handler,
            IAsyncPublisher<GamePauseMessage> gamePausePublisher)
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

        public async UniTask<GameMainLoopResult?> TickAsync(float dt, CancellationToken ct)
        {
            // 時間減少
            _gameMainStore.Dispatch(GameMainStateReducer.DecreaseRemainingTime(dt));

            // 終了条件
            if (GameMainRule.IsTimeUp(_gameMainStore.State.CurrentValue))
                return GameMainLoopResult.Cleared;

            // 入力読み取り
            while (_input.TryDequeue(out var cmd))
            {
                switch (cmd.Type)
                {
                    case GameCommandType.Pause:
                        _onRequestStateChange?.Invoke(GameState.State.Paused);
                        await _gamePausePublisher.PublishAsync(new GamePauseMessage(isPausing: true), ct);
                        break;
                        
                    case GameCommandType.AddScore:
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