using Microsoft.Win32.SafeHandles;

namespace Process.NET.Native.Types
{
    /// <summary>
    ///     Provides a safe handle for a library loaded via LoadLibraryEx.
    /// </summary>
    public class SafeLoadLibrary : SafeHandleMinusOneIsInvalid
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SafeLoadLibrary" /> class.
        /// </summary>
        /// <param name="ownsHandle">
        ///     true to reliably release the handle during the finalization phase; false to prevent reliable
        ///     release (not recommended).
        /// </param>
        public SafeLoadLibrary(bool ownsHandle)
            : base(ownsHandle)
        {
        }

        /// <summary>
        ///     Loads specified library by calling LoadLibraryExW.
        /// </summary>
        /// <param name="library">The library.</param>
        /// <param name="loadLibraryOptions">The load library options.</param>
        /// <returns></returns>
        public static unsafe SafeLoadLibrary LoadLibraryEx(string library, int loadLibraryOptions = 0)
        {
            var result = Kernel32.LoadLibraryExW(library, null, loadLibraryOptions);
            if (result.IsInvalid)
                result.SetHandleAsInvalid();
            return result;
        }

        /// <summary>
        ///     When overridden in a derived class, executes the code required to free the handle.
        /// </summary>
        /// <returns>
        ///     true if the handle is released successfully; otherwise, in the event of a catastrophic failure, false. In this
        ///     case, it generates a releaseHandleFailed MDA Managed Debugging Assistant.
        /// </returns>
        protected override bool ReleaseHandle()
        {
            return Kernel32.FreeLibrary(handle);
        }
    }
}