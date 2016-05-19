using System;
using System.Diagnostics;
using System.Linq;
using Process.NET.Marshaling;

namespace Process.NET.Modules
{
    /// <summary>
    ///     Class representing an injected module in a remote process.
    /// </summary>
    public class InjectedModule : RemoteModule, IDisposableState
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InjectedModule" /> class.
        /// </summary>
        /// <param name="processPlus">The reference of the <see cref="IProcess" /> object.</param>
        /// <param name="module">The native <see cref="ProcessModule" /> object corresponding to the injected module.</param>
        /// <param name="mustBeDisposed">The module will be ejected when the finalizer collects the object.</param>
        public InjectedModule(IProcess processPlus, ProcessModule module, bool mustBeDisposed = true)
            : base(processPlus, module)
        {
            // Save the parameter
            MustBeDisposed = mustBeDisposed;
        }

        /// <summary>
        ///     Gets a value indicating whether the element is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the element must be disposed when the Garbage Collector collects the object.
        /// </summary>
        public bool MustBeDisposed { get; set; }

        /// <summary>
        ///     Releases all resources used by the <see cref="InjectedModule" /> object.
        /// </summary>
        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                // Set the flag to true
                IsDisposed = true;
                // Eject the module
                Process.ModuleFactory.Eject(this);
                // Avoid the finalizer 
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        ///     Frees resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~InjectedModule()
        {
            if (MustBeDisposed)
                Dispose();
        }

        /// <summary>
        ///     Injects the specified module into the address space of the remote process.
        /// </summary>
        /// <param name="memorySharp">The reference of the <see cref="MemorySharp" /> object.</param>
        /// <param name="path">
        ///     The path of the module. This can be either a library module (a .dll file) or an executable module
        ///     (an .exe file).
        /// </param>
        /// <returns>A new instance of the <see cref="InjectedModule" />class.</returns>
        internal static InjectedModule InternalInject(IProcess memorySharp, string path)
        {
            // Call LoadLibraryA remotely
            var thread = memorySharp.ThreadFactory.CreateAndJoin(memorySharp["kernel32"]["LoadLibraryA"].BaseAddress,
                path);
            // Get the inject module
            if (IntPtr.Size == 4)
            {
                if (thread.GetExitCode<IntPtr>() != IntPtr.Zero)
                    return new InjectedModule(memorySharp,
                        memorySharp.ModuleFactory.NativeModules.First(m => m.BaseAddress == thread.GetExitCode<IntPtr>()));
            }
            else if (IntPtr.Size == 8)
            {
                // GetExitCode is not accurate on x64, it returns a DWORD so result is truncated
                // This is a crude method, and probably shouldnt be relied upon.
                if (thread.GetExitCode<IntPtr>() != IntPtr.Zero)
                    return new InjectedModule(memorySharp, memorySharp.ModuleFactory.NativeModules.First(m => m.FileName == path));
            }
            return null;
        }
    }
}