using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer.Unity;
using MessagePipe;
using Game.Domain;
using Game.Application.Contracts;
using Game.Kernel;
using Game.Kernel.Utils.R3;

namespace Game.Application
{
    /// <summary>
    /// ゲームのメインループを実行するクラス。
    /// </summary>
    public sealed class GameMainExecutor : IAsyncStartable
    {
        private readonly Store<GameGlobalState> _globalStore;

        private readonly GameMainLoop _gameMainLoop;
        private readonly InputEmitter _inputEmitter;
        private readonly ISaveService _saveService;
        private readonly ISaveDataApplier _saveDataApplier;
        private readonly IAsyncPublisher<GameStartedMessage> _gameStartedPublisher;
        private readonly IAsyncPublisher<GameFinishedMessage> _gameFinishedPublisher;
        private readonly LoggerBase _logger;

        public GameMainExecutor(
            Store<GameGlobalState> globalStore, 
            GameMainLoop gameMainLoop,
            InputEmitter inputEmitter,
            ISaveService saveService,
            ISaveDataApplier saveDataApplier,
            IAsyncPublisher<GameStartedMessage> gameStartedPublisher,
            IAsyncPublisher<GameFinishedMessage> gameFinishedPublisher,
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
                var saveData = await _saveService.LoadGameDataAsync(ct);
                await _saveDataApplier.ApplyGameDataToStoresAsync(saveData);

                _globalStore.Dispatch(GameGlobalStateReducer.ClearScore());

                // 開始演出
                await _gameStartedPublisher.PublishAsync(new GameStartedMessage(), ct);

                // ゲームメイン実行
                GameMainLoopResult result = await _gameMainLoop.RunAsync(ct);

                //セーブデータ保存
                await _saveService.SaveGameDataAsync(GameSaveData.FromGlobalState(_globalStore.State.CurrentValue), ct);

                // ゲーム終了メッセージ発行
                await _gameFinishedPublisher.PublishAsync(new GameFinishedMessage(result), ct);
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