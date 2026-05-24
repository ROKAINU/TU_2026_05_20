using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer.Unity;
using Game.Domain;
using Game.Application.Contracts;

namespace Game.Application.Runner
{
    /// <summary>
    /// タイトルの開始処理を実行するクラス。
    /// </summary>
    public sealed class TitleExecutor : IAsyncStartable
    {
        private readonly IEventPublisher<TitleStartedMessage> _titleStartedPublisher;

        public TitleExecutor(IEventPublisher<TitleStartedMessage> titleStartedPublisher)
        {
            _titleStartedPublisher = titleStartedPublisher;
        }

        /// <summary>
        /// タイトルの開始処理を実行する。ここでは、タイトル開始のメッセージを発行する。
        /// </summary>
        /// <param name="cancellationToken">キャンセル用のトークン</param>
        public async UniTask StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _titleStartedPublisher.PublishAsync(new TitleStartedMessage(), cancellationToken).AsUniTask();
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}