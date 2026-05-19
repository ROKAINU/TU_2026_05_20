#nullable enable
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Kernel.Utils.Cysharp
{
    public sealed class AsyncCommandQueue<TCommand> : IAsyncCommandQueue<TCommand>
    {
        private readonly Queue<TCommand> _queue = new();
        private UniTaskCompletionSource<bool>? _signal;

        public void Enqueue(in TCommand command)
        {
            _queue.Enqueue(command);
            _signal?.TrySetResult(true);
        }

        public void Clear() => _queue.Clear();

        public bool TryDequeue(out TCommand command)
        {
            if (_queue.Count > 0)
            {
                command = _queue.Dequeue();
                return true;
            }

            command = default!;
            return false;
        }

        public async UniTask<TCommand> NextAsync(CancellationToken token)
        {
            while (_queue.Count == 0)
            {
                _signal = new UniTaskCompletionSource<bool>();
                using (token.Register(() => _signal.TrySetCanceled(token)))
                    await _signal.Task;
                _signal = null;
            }

            return _queue.Dequeue();
        }
    }
}