using UnityEngine;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using MessagePipe.VContainer;
using Game.Domain;
using Game.Application;
using Game.Application.Contracts;
using Game.Infrastructure;
using Game.Infrastructure.Save;
using Game.Presentation;
using Game.Presentation.View;
using Game.Kernel.Utils.R3;
using Game.Kernel.Utils.Cysharp;

namespace Game.Infrastructure.LifetimeScopes
{
    public class GameMainLifetimeScope : LifetimeScope
    {
        [SerializeField] private BGMSO bgmSO;
        [SerializeField] private GameMainUIInstance gameMainUIInstance;

        protected override void Configure(IContainerBuilder builder)
        {
            // MessagePipe 設定
            var messagePipeOptions = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<GameStartedMessage>(messagePipeOptions);
            builder.RegisterMessageBroker<GamePauseMessage>(messagePipeOptions);
            builder.RegisterMessageBroker<GameFinishedMessage>(messagePipeOptions);

            builder.Register<IAsyncCommandQueue<GameCommand>, AsyncCommandQueue<GameCommand>>(Lifetime.Scoped);
            builder.Register<ICommandHandler<GameCommand>, GameCommandHandler>(Lifetime.Scoped);

            // ========================================
            // Domain層
            // ========================================
            builder.Register<Store<GameMainState>>(Lifetime.Scoped).WithParameter(GameMainState.Default);

            // ========================================
            // Application層
            // ========================================
            builder.Register<GameState>(Lifetime.Scoped).WithParameter(GameState.State.Playing);
            builder.Register<PausedPhase>(Lifetime.Scoped);
            builder.Register<PlayingPhase>(Lifetime.Scoped);
            builder.Register<GameMainLoop>(Lifetime.Scoped);
            builder.RegisterEntryPoint<GameMainExecutor>(Lifetime.Scoped);

            // ========================================
            // Infrastructure層
            // ========================================
            builder.Register<IInputService, RealInputService>(Lifetime.Scoped);
            builder.Register<InputEmitter>(Lifetime.Scoped);

            // ========================================
            // Presentation層
            // ========================================
            builder.RegisterInstance(bgmSO);
            builder.RegisterInstance(gameMainUIInstance);
            builder.RegisterEntryPoint<GameMainPresenter>(Lifetime.Scoped);
            builder.Register<GameMainCommandBinder>(Lifetime.Scoped);
        }
    }
}