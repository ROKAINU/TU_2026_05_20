using VContainer;
using MessagePipe;
using MessagePipe.VContainer;
using Game.Application.Contracts;
using Game.Infrastructure.Messaging;

namespace Game.Infrastructure
{
    public static class MessagePipeExtensions
    {
        /// <summary>
        /// MessageBroker と EventPublisher を登録
        /// </summary>
        public static void RegisterEvent<T>(
            this IContainerBuilder builder,
            MessagePipeOptions options)
        {
            builder.RegisterMessageBroker<T>(options);

            builder.Register<IEventPublisher<T>, EventPublisher<T>>(Lifetime.Singleton);
        }
    }
}