using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Process.NET.Marshaling;
using Process.NET.Native.Types;
using Process.NET.Utilities;

namespace Process.NET.Threads
{
    /// <summary>
    ///     Class providing tools for manipulating threads.
    /// </summary>
    public class ThreadFactory : IThreadFactory
    {
        /// <summary>
        ///     The reference of the <see cref="Process" /> object.
        /// </summary>
        protected readonly IProcess Process;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ThreadFactory" /> class.
        /// </summary>
        /// <param name="process">The reference of the <see cref="Process" /> object.</param>
        public ThreadFactory(IProcess process)
        {
            // Save the parameter
            Process = process;
        }

        /// <summary>
        ///     Gets the main thread of the remote process.
        /// </summary>
        public IRemoteThread MainThread
        {
            get
            {
                return new RemoteThread(Process,
                    NativeThreads.Aggregate((current, next) => next.StartTime < current.StartTime ? next : current));
            }
        }

        /// <summary>
        ///     Gets the native threads from the remote process.
        /// </summary>
        public IEnumerable<ProcessThread> NativeThreads
        {
            get
            {
                // Refresh the process info
                Process.Native.Refresh();
                // Enumerates all threads
                return Process.Native.Threads.Cast<ProcessThread>();
            }
        }

        /// <summary>
        ///     Gets the threads from the remote process.
        /// </summary>
        public IEnumerable<IRemoteThread> RemoteThreads
        {
            get { return NativeThreads.Select(t => new RemoteThread(Process, t)); }
        }

        /// <summary>
        ///     Gets the thread corresponding to an id.
        /// </summary>
        /// <param name="threadId">The unique identifier of the thread to get.</param>
        /// <returns>A new instance of a <see cref="RemoteThread" /> class.</returns>
        public IRemoteThread this[int threadId]
        {
            get { return new RemoteThread(Process, NativeThreads.First(t => t.Id == threadId)); }
        }

        /// <summary>
        ///     Creates a thread that runs in the remote process.
        /// </summary>
        /// <param name="address">
        ///     A pointer to the application-defined function to be executed by the thread and represents
        ///     the starting address of the thread in the remote process.
        /// </param>
        /// <param name="parameter">A variable to be passed to the thread function.</param>
        /// <param name="isStarted">Sets if the thread must be started just after being created.</param>
        /// <returns>A new instance of the <see cref="RemoteThread" /> class.</returns>
        public IRemoteThread Create(IntPtr address, dynamic parameter, bool isStarted = true)
        {
            // Marshal the parameter
            var marshalledParameter = MarshalValue.Marshal(Process, parameter);

            //Create the thread
            var ret = ThreadHelper.NtQueryInformationThread(
                ThreadHelper.CreateRemoteThread(Process.Handle, address, marshalledParameter.Reference,
                    ThreadCreationFlags.Suspended));

            // Find the managed object corresponding to this thread
            // TODO (int) cast may be unnecessary and/or problematic. Suggest coming back for proper fix later
            var result = new RemoteThread(Process, NativeThreads.First(t => t.Id == (int)ret.ClientId.UniqueThread),
                marshalledParameter);

            // If the thread must be started
            if (isStarted)
                result.Resume();
            return result;
        }

        /// <summary>
        ///     Creates a thread that runs in the remote process.
        /// </summary>
        /// <param name="address">
        ///     A pointer to the application-defined function to be executed by the thread and represents
        ///     the starting address of the thread in the remote process.
        /// </param>
        /// <param name="isStarted">Sets if the thread must be started just after being created.</param>
        /// <returns>A new instance of the <see cref="RemoteThread" /> class.</returns>
        public IRemoteThread Create(IntPtr address, bool isStarted = true)
        {
            //Create the thread
            var ret = ThreadHelper.NtQueryInformationThread(
                ThreadHelper.CreateRemoteThread(Process.Handle, address, IntPtr.Zero,
                    ThreadCreationFlags.Suspended));

            // Find the managed object corresponding to this thread
            // TODO (int) cast may be unnecessary and/or problematic. Suggest coming back for proper fix later
            var result = new RemoteThread(Process, NativeThreads.First(t => t.Id == (int)ret.ClientId.UniqueThread));

            // If the thread must be started
            if (isStarted)
                result.Resume();
            return result;
        }

        /// <summary>
        ///     Creates a thread in the remote process and blocks the calling thread until the thread terminates.
        /// </summary>
        /// <param name="address">
        ///     A pointer to the application-defined function to be executed by the thread and represents
        ///     the starting address of the thread in the remote process.
        /// </param>
        /// <param name="parameter">A variable to be passed to the thread function.</param>
        /// <returns>A new instance of the <see cref="RemoteThread" /> class.</returns>
        public IRemoteThread CreateAndJoin(IntPtr address, dynamic parameter)
        {
            // Create the thread
            var ret = Create(address, parameter);
            // Wait the end of the thread
            ret.Join();
            // Return the thread
            return ret;
        }

        /// <summary>
        ///     Creates a thread in the remote process and blocks the calling thread until the thread terminates.
        /// </summary>
        /// <param name="address">
        ///     A pointer to the application-defined function to be executed by the thread and represents
        ///     the starting address of the thread in the remote process.
        /// </param>
        /// <returns>A new instance of the <see cref="RemoteThread" /> class.</returns>
        public IRemoteThread CreateAndJoin(IntPtr address)
        {
            // Create the thread
            var ret = Create(address);
            // Wait the end of the thread
            ret.Join();
            // Return the thread
            return ret;
        }

        /// <summary>
        ///     Releases all resources used by the <see cref="ThreadFactory" /> object.
        /// </summary>
        public void Dispose()
        {
            // Nothing to dispose... yet
        }

        /// <summary>
        ///     Gets a thread by its id in the remote process.
        /// </summary>
        /// <param name="id">The id of the thread.</param>
        /// <returns>A new instance of the <see cref="RemoteThread" /> class.</returns>
        public IRemoteThread GetThreadById(int id)
        {
            return new RemoteThread(Process, NativeThreads.First(t => t.Id == id));
        }

        /// <summary>
        ///     Resumes all threads.
        /// </summary>
        public void ResumeAll()
        {
            foreach (var thread in RemoteThreads)
                thread.Resume();
        }

        /// <summary>
        ///     Suspends all threads.
        /// </summary>
        public void SuspendAll()
        {
            foreach (var thread in RemoteThreads)
                thread.Suspend();
        }
    }
}