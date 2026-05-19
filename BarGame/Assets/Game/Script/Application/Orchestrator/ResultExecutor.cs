using Game.Domain;
using VContainer.Unity;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using MessagePipe;
using Game.Kernel.Utils.R3;

namespace Game.Application
{
    /// <summary>
    /// リザルトの開始処理を実行するクラス。
    /// </summary>
    public sealed class ResultExecutor : IAsyncStartable
    {
        private readonly IAsyncPublisher<ResultStartedMessage> _resultStartedPublisher;

        public ResultExecutor(IAsyncPublisher<ResultStartedMessage> resultStartedPublisher)
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
                await _resultStartedPublisher.PublishAsync(new ResultStartedMessage(), cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}