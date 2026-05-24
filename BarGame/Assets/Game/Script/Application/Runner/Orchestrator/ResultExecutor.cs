using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer.Unity;
using Game.Domain;
using Game.Application.Contracts;

namespace Game.Application.Runner
{
    /// <summary>
    /// リザルトの開始処理を実行するクラス。
    /// </summary>
    public sealed class ResultExecutor : IAsyncStartable
    {
        private readonly IEventPublisher<ResultStartedMessage> _resultStartedPublisher;

        public ResultExecutor(IEventPublisher<ResultStartedMessage> resultStartedPublisher)
        {
            _resultStartedPublisher = resultStartedPublisher;
        }

        /// <summary>
        /// リザルトの開始処理を実行する。ここでは、リザルト開始のメッセージを発行する。
        /// </summary>
        /// <param name="cancellationToken">キャンセル用のトークン</param>
        public async UniTask StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _resultStartedPublisher.PublishAsync(new ResultStartedMessage(), cancellationToken).AsUniTask();
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}