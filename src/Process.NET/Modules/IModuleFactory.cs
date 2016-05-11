using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Process.NET.Modules
{
    public interface IModuleFactory : IDisposable
    {
        IProcessModule this[string moduleName] { get; }

        IEnumerable<InjectedModule> InjectedModules { get; }
        IProcessModule MainModule { get; }
        IEnumerable<IProcessModule> RemoteModules { get; }
        IEnumerable<ProcessModule> NativeModules { get; }
        void Eject(string moduleName);
        void Eject(IProcessModule module);
        InjectedModule Inject(string path, bool mustBeDisposed = true);
    }
}