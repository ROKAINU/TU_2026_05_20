using UnityEngine;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using MessagePipe.VContainer;
using Game.Domain;
using Game.Application;
using Game.Presentation;
using Game.Presentation.View;

namespace Game.Infrastructure.LifetimeScopes
{
    public class ResultLifetimeScope : LifetimeScope
    {
        [SerializeField] private ResultUIInstance resultUIInstance;
        [SerializeField] private BGMSO bgmSO;
        [SerializeField] private SceneTransitionUI sceneTransitionUI;

        protected override void Configure(IContainerBuilder builder)
        {
            // MessagePipe 設定
            var messagePipeOptions = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<ResultStartedMessage>(messagePipeOptions);

            // ========================================
            // Application層
            // ========================================
            builder.RegisterEntryPoint<ResultPresenter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<ResultExecutor>(Lifetime.Scoped);

            // ========================================
            // Presentation層
            // ========================================
            builder.RegisterInstance(resultUIInstance);
            builder.RegisterInstance(bgmSO);
            builder.RegisterComponent(sceneTransitionUI);
        }
    }
}