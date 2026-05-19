using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Kernel.Utils.Cysharp
{
    public interface IAsyncCommandQueue<TCommand>
    {
        void Enqueue(in TCommand command);
        UniTask<TCommand> NextAsync(CancellationToken token);
        bool TryDequeue(out TCommand command);
        void Clear(); // Pause時に捨てたい等のため
    }
}