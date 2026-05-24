using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Game.Application.Contracts;

namespace Game.Infrastructure.Messaging
{
    public sealed class EventPublisher<TMessage> : IEventPublisher<TMessage>
    {
        private readonly IAsyncPublisher<TMessage> _publisher;

        public EventPublisher(IAsyncPublisher<TMessage> publisher)
            => _publisher = publisher;

        public Task PublishAsync(TMessage message, CancellationToken ct = default)
            => _publisher.PublishAsync(message, ct).AsTask();
    }
}