using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Process.NET.Threads
{
    public interface IThreadFactory : IDisposable
    {
        IRemoteThread this[int threadId] { get; }

        IRemoteThread MainThread { get; }
        IEnumerable<ProcessThread> NativeThreads { get; }
        IEnumerable<IRemoteThread> RemoteThreads { get; }

        IRemoteThread Create(IntPtr address, bool isStarted = true);
        IRemoteThread Create(IntPtr address, dynamic parameter, bool isStarted = true);
        IRemoteThread CreateAndJoin(IntPtr address);
        IRemoteThread CreateAndJoin(IntPtr address, dynamic parameter);
        IRemoteThread GetThreadById(int id);
        void ResumeAll();
        void SuspendAll();
    }
}