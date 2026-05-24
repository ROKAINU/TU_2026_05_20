using System;
using System.Runtime.CompilerServices;

namespace Game.Kernel
{
    public interface IStore<T> : IDisposable
    {
        T CurrentState { get; }

        void Dispatch(
            Func<T, T> reducer,
            [CallerMemberName] string callerName = "",
            [CallerFilePath]   string callerFile = "");
    }
}