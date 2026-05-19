using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Kernel.Utils.Cysharp
{
    public interface ICommandHandler<TCommand>
    {
        UniTask HandleAsync(TCommand command, CancellationToken token);
    }
}