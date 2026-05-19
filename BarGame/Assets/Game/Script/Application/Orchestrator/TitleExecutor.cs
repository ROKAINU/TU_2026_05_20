using UnityEngine;
using VContainer.Unity;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using MessagePipe;
using Game.Domain;
using Game.Kernel.Utils.R3;

namespace Game.Application
{
    /// <summary>
    /// タイトルの開始処理を実行するクラス。
    /// </summary>
    public sealed class TitleExecutor : IAsyncStartable
    {
        private readonly IAsyncPublisher<TitleStartedMessage> _titleStartedPublisher;

        public TitleExecutor(IAsyncPublisher<TitleStartedMessage> titleStartedPublisher)
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
                await _titleStartedPublisher.PublishAsync(new TitleStartedMessage(), cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}