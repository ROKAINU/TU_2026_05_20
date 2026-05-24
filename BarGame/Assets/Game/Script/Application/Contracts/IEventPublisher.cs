using System.Threading;
using System.Threading.Tasks;

namespace Game.Application.Contracts
{
    public interface IEventPublisher<TMessage>
    {
        Task PublishAsync(TMessage message, CancellationToken ct = default);
    }
}