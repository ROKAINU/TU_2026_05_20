using System;

namespace Game.Kernel.Utils.Cysharp
{
    public interface ICommandEmitter : IDisposable
    {
        void Enable();
        void Disable();
    }
}