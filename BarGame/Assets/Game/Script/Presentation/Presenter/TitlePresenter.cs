#nullable enable
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using VContainer.Unity;
using Game.Domain;
using Game.Infrastructure;
using Game.Kernel;
using Game.Kernel.Utils.R3;

namespace Game.Presentation
{
    /// <summary>
    /// タイトル画面表示制御
    /// 責務: BGM、HUD、シーン遷移
    /// </summary>
    public sealed class TitlePresenter : IInitializable, IDisposable
    {
        private readonly BGMPlayer _bgmPlayer;
        private readonly BGMSO _bgmSO;

        private readonly IAsyncSubscriber<TitleStartedMessage> _titleStartedSubscriber;
        private IDisposable? _titleStartedDisposable;
        private bool _disposed;

        public TitlePresenter(
            BGMPlayer bgmPlayer,
            BGMSO bgmSO,
            IAsyncSubscriber<TitleStartedMessage> titleStartedSubscriber)
        {
            _bgmPlayer = bgmPlayer;
            _bgmSO = bgmSO;
            
            _titleStartedSubscriber = titleStartedSubscriber;
        }

        /// <summary>
        /// IAsyncStartable より先に呼ばれる。ここで MessagePipe に購読を登録する。
        /// </summary>
        public void Initialize()
        {
            _titleStartedDisposable = _titleStartedSubscriber.Subscribe(OnTitleStartedAsync);
        }
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _titleStartedDisposable?.Dispose();
            _titleStartedDisposable = null;
        }

        private UniTask OnTitleStartedAsync(TitleStartedMessage msg, CancellationToken cancellationToken)
        {
            _bgmPlayer.PlayBGM(_bgmSO.bgmClips[0]);
            return UniTask.CompletedTask;
        }

        public UniTask DisposeAsync()
        {
            Dispose();
            return UniTask.CompletedTask;
        }
    }
}