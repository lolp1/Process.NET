namespace Process.NET.Threads
{
    /// <summary>
    ///     Class containing a frozen thread. If an instance of this class is disposed, its associated thread is resumed.
    /// </summary>
    public class FrozenThread : IFrozenThread
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FrozenThread" /> class.
        /// </summary>
        /// <param name="thread">The frozen thread.</param>
        public FrozenThread(IRemoteThread thread)
        {
            // Save the parameter
            Thread = thread;
        }

        /// <summary>
        ///     The frozen thread.
        /// </summary>
        public IRemoteThread Thread { get; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// TODO Edit XML Comment Template for Dispose
        public virtual void Dispose()
        {
            // Unfreeze the thread
            Thread.Resume();
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return $"Id = {Thread.Id}";
        }
    }
}