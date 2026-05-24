#nullable enable
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using VContainer.Unity;
using Game.Domain;
using Game.Infrastructure;
using Game.Presentation.SceneTransition;
using Game.Presentation.View;

namespace Game.Presentation
{
    /// <summary>
    /// ゲームメイン表示制御
    /// 責務: BGM、HUD、シーン遷移
    /// </summary>
    public sealed class GameMainPresenter : IInitializable, IDisposable
    {
        private readonly BGMPlayer _bgmPlayer;
        private readonly BGMSO _bgmSO;
        private readonly GameMainUIInstance _gameMainUIInstance;
        private readonly SceneTransitioner _sceneTransitioner;
        private readonly GameMainCommandBinder _commandBinder;
        private readonly IAsyncSubscriber<GameStartedMessage> _gameStartedSubscriber;
        private readonly IAsyncSubscriber<GamePauseMessage> _gamePauseSubscriber;
        private readonly IAsyncSubscriber<GameFinishedMessage> _gameFinishedSubscriber;

        private IDisposable? _gameStartedDisposable;
        private IDisposable? _gamePauseDisposable;
        private IDisposable? _gameFinishedDisposable;
        private bool _disposed;

        private static readonly SceneTransitioner.TransitionOptions TransitionOpt =
            new SceneTransitioner.TransitionOptions(fadeDuration: 1f);

        public GameMainPresenter(
            BGMPlayer bgmPlayer,
            BGMSO bgmSO,
            GameMainUIInstance gameMainUIInstance,
            SceneTransitioner sceneTransitioner,
            GameMainCommandBinder commandBinder,
            IAsyncSubscriber<GameStartedMessage> gameStartedSubscriber,
            IAsyncSubscriber<GamePauseMessage> gamePauseSubscriber,
            IAsyncSubscriber<GameFinishedMessage> gameFinishedSubscriber)
        {
            _bgmPlayer = bgmPlayer;
            _bgmSO = bgmSO;
            _gameMainUIInstance = gameMainUIInstance;
            _sceneTransitioner = sceneTransitioner;
            _commandBinder = commandBinder;
            _gameStartedSubscriber = gameStartedSubscriber;
            _gamePauseSubscriber = gamePauseSubscriber;
            _gameFinishedSubscriber = gameFinishedSubscriber;
        }

        /// <summary>
        /// IAsyncStartable より先に呼ばれる。ここで MessagePipe に購読を登録する。
        /// </summary>
        public void Initialize()
        {
            _gameStartedDisposable = _gameStartedSubscriber.Subscribe(OnGameStartedAsync);
            _gamePauseDisposable = _gamePauseSubscriber.Subscribe(OnGamePausedAsync);
            _gameFinishedDisposable = _gameFinishedSubscriber.Subscribe(OnGameFinishedAsync);
        }
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _gameStartedDisposable?.Dispose();
            _gamePauseDisposable?.Dispose();
            _gameFinishedDisposable?.Dispose();
            _gameStartedDisposable = null;
            _gamePauseDisposable = null;
            _gameFinishedDisposable = null;
        }

        private UniTask OnGameStartedAsync(GameStartedMessage msg, CancellationToken cancellationToken)
        {
            _commandBinder.Bind();
            _bgmPlayer.PlayBGM(_bgmSO.bgmClips[0]);
            return UniTask.CompletedTask;
        }

        private UniTask OnGamePausedAsync(GamePauseMessage msg, CancellationToken cancellationToken)
        {
            if (msg.IsPausing)
            {
                // 一時停止の処理
                _gameMainUIInstance.pauseWindow.SetActive(true);
                _gameMainUIInstance.pauseButton.gameObject.SetActive(false);
            }
            else
            {
                // 一時停止解除の処理
                _gameMainUIInstance.pauseWindow.SetActive(false);
                _gameMainUIInstance.pauseButton.gameObject.SetActive(true);
            }
            return UniTask.CompletedTask;
        }

        private UniTask OnGameFinishedAsync(GameFinishedMessage msg, CancellationToken cancellationToken) =>
            HandleGameFinishedAsync(msg.Result, cancellationToken);

        private async UniTask HandleGameFinishedAsync(GameMainLoopResult result, CancellationToken cancellationToken)
        {
            switch (result)
            {
                case GameMainLoopResult.Canceled:
                    return;

                case GameMainLoopResult.GameFinished:
                case GameMainLoopResult.ToResult:
                    await _sceneTransitioner.TransitionToAsync(SceneId.Result, TransitionOpt);
                    break;

                case GameMainLoopResult.Cleared:
                    await _sceneTransitioner.TransitionToAsync(SceneId.Result, TransitionOpt);
                    break;

                case GameMainLoopResult.ToTitle:
                    await _sceneTransitioner.TransitionToAsync(SceneId.Title, TransitionOpt);
                    break;

                case GameMainLoopResult.Restart:
                    await _sceneTransitioner.TransitionToAsync(SceneId.Main, TransitionOpt);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(result));
            }
        }

        public UniTask DisposeAsync()
        {
            Dispose();
            return UniTask.CompletedTask;
        }
    }
}