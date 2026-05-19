using UnityEngine;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using MessagePipe.VContainer;
using Game.Domain;
using Game.Application;
using Game.Presentation;

namespace Game.Infrastructure.LifetimeScopes
{
    public class TitleLifetimeScope : LifetimeScope
    {
        [SerializeField] private BGMSO bgmSO;

        protected override void Configure(IContainerBuilder builder)
        {
            // MessagePipe 設定
            var messagePipeOptions = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<TitleStartedMessage>(messagePipeOptions);
        
            // ========================================
            // Application層
            // ========================================
            builder.RegisterEntryPoint<TitlePresenter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<TitleExecutor>(Lifetime.Scoped);

            // ========================================
            // Presentation層
            // ========================================
            builder.RegisterInstance(bgmSO);
        }
    }
}