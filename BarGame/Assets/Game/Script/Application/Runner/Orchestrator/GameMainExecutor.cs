using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer.Unity;
using Game.Domain;
using Game.Application.Contracts;
using Game.Kernel;
using Game.Kernel.Utils.Cysharp;

namespace Game.Application.Runner
{
    /// <summary>
    /// ゲームのメインループを実行するクラス。
    /// </summary>
    public sealed class GameMainExecutor : IAsyncStartable
    {
        private readonly IStore<GameGlobalState> _globalStore;

        private readonly GameMainLoop _gameMainLoop;
        private readonly ICommandEmitter _inputEmitter;
        private readonly ISaveService _saveService;
        private readonly ISaveDataApplier _saveDataApplier;
        private readonly IEventPublisher<GameStartedMessage> _gameStartedPublisher;
        private readonly IEventPublisher<GameFinishedMessage> _gameFinishedPublisher;
        private readonly LoggerBase _logger;

        public GameMainExecutor(
            IStore<GameGlobalState> globalStore, 
            GameMainLoop gameMainLoop,
            ICommandEmitter inputEmitter,
            ISaveService saveService,
            ISaveDataApplier saveDataApplier,
            IEventPublisher<GameStartedMessage> gameStartedPublisher,
            IEventPublisher<GameFinishedMessage> gameFinishedPublisher,
            LoggerBase logger)
        {
            _globalStore = globalStore;
            _gameMainLoop = gameMainLoop;
            _inputEmitter = inputEmitter;
            _saveService = saveService;
            _saveDataApplier = saveDataApplier;
            _gameStartedPublisher = gameStartedPublisher;
            _gameFinishedPublisher = gameFinishedPublisher;
            _logger = logger;
        }

        /// <summary>
        /// ゲームのメインループを実行する。ゲームの状態を監視し、入力を処理し、ゲームの状態を更新する。
        /// </summary>
        /// <param name="ct">キャンセル用のトークン</param>
        public async UniTask StartAsync(CancellationToken ct)
        {
            _inputEmitter.Enable();
            try
            {
                // セットアップ
                var saveData = await _saveService.LoadGameDataAsync(ct).AsUniTask();
                await _saveDataApplier.ApplyGameDataToStoresAsync(saveData);

                _globalStore.Dispatch(GameGlobalStateReducer.ClearScore());

                // 開始演出
                await _gameStartedPublisher.PublishAsync(new GameStartedMessage(), ct).AsUniTask();

                // ゲームメイン実行
                GameMainLoopResult result = await _gameMainLoop.RunAsync(ct);

                //セーブデータ保存
                await _saveService.SaveGameDataAsync(GameSaveData.FromGlobalState(_globalStore.CurrentState), ct).AsUniTask();

                // ゲーム終了メッセージ発行
                await _gameFinishedPublisher.PublishAsync(new GameFinishedMessage(result), ct).AsUniTask();
            }
            catch (OperationCanceledException)
            {
                _logger.LogDebug("GameMainExecutor was canceled.");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "An error occurred during GameMainExecutor execution.");
            }
            finally
            {
                _inputEmitter.Disable();
            }
        }
    }
}