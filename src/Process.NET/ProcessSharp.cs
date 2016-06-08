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
        /// Initializes a new instance of the <see cref="ProcessSharp"/> class.
        /// </summary>
        /// <param name="processName">Name of the process.</param>
        public ProcessSharp(string processName) : this(ProcessHelper.FromName(processName))
        {           
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessSharp"/> class.
        /// </summary>
        /// <param name="processId">The process id of the process to open with all rights.</param>
        public ProcessSharp(int processId) : this(ProcessHelper.FromProcessId(processId))
        {
        }

        /// <summary>
        ///     Class for reading and writing memory.
        /// </summary>
        public IMemory Memory { get; set; }

        /// <summary>
        ///     Provide access to the opened process.
        /// </summary>
        public System.Diagnostics.Process Native { get; set; }

        /// <summary>
        ///     The process handle opened with all rights.
        /// </summary>
        public SafeMemoryHandle Handle { get; set; }

        /// <summary>
        ///     Factory for manipulating threads.
        /// </summary>
        public IThreadFactory ThreadFactory { get; set; }

        /// <summary>
        ///     Factory for manipulating modules and libraries.
        /// </summary>
        public IModuleFactory ModuleFactory { get; set; }

        /// <summary>
        ///     Factory for manipulating memory space.
        /// </summary>
        public IMemoryFactory MemoryFactory { get; set; }

        /// <summary>
        ///     Factory for manipulating windows.
        /// </summary>
        public IWindowFactory WindowFactory { get; set; }

        /// <summary>
        /// Gets the <see cref="IProcessModule"/> with the specified module name.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <returns>IProcessModule.</returns>
        public IProcessModule this[string moduleName] => ModuleFactory[moduleName];

        /// <summary>
        /// Gets the <see cref="IPointer"/> with the specified address.
        /// </summary>
        /// <param name="intPtr">The address the pointer is located at in memory.</param>
        /// <returns>IPointer.</returns>
        public IPointer this[IntPtr intPtr] => new MemoryPointer(this, intPtr);

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public virtual void Dispose()
        {
            ThreadFactory?.Dispose();
            ModuleFactory?.Dispose();
            MemoryFactory?.Dispose();
            WindowFactory?.Dispose();
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