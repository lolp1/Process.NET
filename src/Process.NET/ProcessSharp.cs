using System;
using System.Diagnostics;
using Process.NET.Memory;
using Process.NET.Modules;
using Process.NET.Native.Types;
using Process.NET.Threads;
using Process.NET.Utilities;
using Process.NET.Windows;

namespace Process.NET
{
    /// <summary>
    ///     A class that offsers several tools to interact with a process.
    /// </summary>
    /// <seealso cref="IProcess" />
    public class ProcessSharp : IProcess
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ProcessSharp" /> class.
        /// </summary>
        /// <param name="native">The native process.</param>
        public ProcessSharp(System.Diagnostics.Process native)
        {
            native.EnableRaisingEvents = true;

            native.Exited += (s, e) =>
            {
                ProcessExited?.Invoke(s, e);
                HandleProcessExiting();
            };

            Native = native;
            Handle = MemoryHelper.OpenProcess(ProcessAccessFlags.AllAccess, Native.Id);

            native.ErrorDataReceived += OutputDataReceived;
            native.OutputDataReceived += OutputDataReceived;

            ThreadFactory = new ThreadFactory(this);
            ModuleFactory = new ModuleFactory(this);
            MemoryFactory = new MemoryFactory(this);
            WindowFactory = new WindowFactory(this);
        }

        /// <summary>
        ///     Class for reading and writing memory.
        /// </summary>
        public IMemory Memory { get; set; }

        /// <summary>
        ///     Provide access to the opened process.
        /// </summary>
        public System.Diagnostics.Process Native { get; }

        /// <summary>
        ///     The process handle opened with all rights.
        /// </summary>
        public SafeMemoryHandle Handle { get; }

        IMemory IProcess.Memory => Memory;

        /// <summary>
        ///     Factory for manipulating threads.
        /// </summary>
        public IThreadFactory ThreadFactory { get; }

        /// <summary>
        ///     Factory for manipulating modules and libraries.
        /// </summary>
        public IModuleFactory ModuleFactory { get; }

        /// <summary>
        ///     Factory for manipulating memory space.
        /// </summary>
        public IMemoryFactory MemoryFactory { get; }

        /// <summary>
        ///     Factory for manipulating windows.
        /// </summary>
        public IWindowFactory WindowFactory { get; }

        public IProcessModule this[string moduleName] => ModuleFactory[moduleName];

        public IPointer this[IntPtr intPtr] => new MemoryPointer(this, intPtr);

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public virtual void Dispose()
        {
            // TODO Consider adding a null check here, or a nasty crash deadlock can make it in here occasionally.
            ThreadFactory.Dispose();
            ModuleFactory.Dispose();
            MemoryFactory.Dispose();
            WindowFactory.Dispose();
            Handle.Close();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Handles the process exiting.
        /// </summary>
        /// <remarks>Created 2012-02-15</remarks>
        protected virtual void HandleProcessExiting()
        {
        }

        /// <summary>
        ///     Event queue for all listeners interested in ProcessExited events.
        /// </summary>
        public event EventHandler ProcessExited;

        private static void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Trace.Write(e.Data);
        }

        ~ProcessSharp()
        {
            Dispose();
        }
    }
}