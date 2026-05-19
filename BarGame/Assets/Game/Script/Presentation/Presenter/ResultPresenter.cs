#nullable enable
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using VContainer.Unity;
using Game.Domain;
using Game.Presentation.View;
using Game.Kernel;
using Game.Kernel.Utils.R3;

using UnityEngine;

namespace Game.Presentation
{
    /// <summary>
    /// リザルト画面表示制御
    /// 責務: スコア表示、BGM
    /// </summary>
    public sealed class ResultPresenter : IInitializable, IDisposable
    {
        private readonly BGMPlayer _bgmPlayer;
        private readonly BGMSO _bgmSO;
        private readonly ResultUIInstance _resultUIInstance;
        private readonly Store<GameGlobalState> _globalStore;

        private readonly IAsyncSubscriber<ResultStartedMessage> _resultStartedSubscriber;
        private IDisposable? _resultStartedDisposable;
        private bool _disposed;

        public ResultPresenter(
            ResultUIInstance resultUIInstance,
            BGMPlayer bgmPlayer,
            BGMSO bgmSO,
            Store<GameGlobalState> globalStore,
            IAsyncSubscriber<ResultStartedMessage> resultStartedSubscriber)
        {
            _resultUIInstance = resultUIInstance;
            _bgmPlayer = bgmPlayer;
            _bgmSO = bgmSO;
            _globalStore = globalStore;
            _resultStartedSubscriber = resultStartedSubscriber;
        }

        /// <summary>
        /// IAsyncStartable より先に呼ばれる。ここで MessagePipe に購読を登録する。
        /// </summary>
        public void Initialize()
        {
            _resultStartedDisposable = _resultStartedSubscriber.Subscribe(OnResultStartedAsync);
        }
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _resultStartedDisposable?.Dispose();
            _resultStartedDisposable = null;
        }

        private UniTask OnResultStartedAsync(ResultStartedMessage msg, CancellationToken cancellationToken)
        {
            _bgmPlayer.PlayBGM(_bgmSO.bgmClips[0]);
            _resultUIInstance.scoreText.text = "Score : " + _globalStore.State.CurrentValue.CurrentScore.Value.ToString();
            _resultUIInstance.highScoreText.text = "High Score : " + _globalStore.State.CurrentValue.HighScore.Value.ToString();   
            return UniTask.CompletedTask;
        }

        public UniTask DisposeAsync()
        {
            Dispose();
            return UniTask.CompletedTask;
        }
    }
}