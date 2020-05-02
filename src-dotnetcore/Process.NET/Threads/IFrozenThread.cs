using System;

namespace Process.NET.Threads
{
    public interface IFrozenThread : IDisposable
    {
        IRemoteThread Thread { get; }
    }
}