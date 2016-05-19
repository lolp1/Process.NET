using System;
using System.Runtime.InteropServices;

namespace Process.NET.Utilities
{
    public static class UnsafeMemoryExtensions
    {
        // TODO add summary to this
        public static IntPtr GetVtableIntPtr(this IntPtr intPtr, int functionIndex)
        {
            var vftable = MemoryHelper.InternalRead<IntPtr>(intPtr);
            return MemoryHelper.InternalRead<IntPtr>(vftable + functionIndex * IntPtr.Size);
        }
        /// <summary>
        ///     Converts an unmanaged delegate to a function pointer.
        /// </summary>
        public static IntPtr ToFunctionPtr(this Delegate d)
        {
            return Marshal.GetFunctionPointerForDelegate(d);
        }

        /// <summary>
        ///     Converts an unmanaged function pointer to the given delegate type.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="addr">Where address of the function.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">
        ///     This operation can only convert to delegates adorned with the
        ///     UnmanagedFunctionPointerAttribute
        /// </exception>
        /// TODO Edit XML Comment Template for ToDelegate`1
        public static T ToDelegate<T>(this IntPtr addr) where T : class
        {
            if (typeof (T).GetCustomAttributes(typeof (UnmanagedFunctionPointerAttribute), true).Length == 0)
                throw new InvalidOperationException(
                    "This operation can only convert to delegates adorned with the UnmanagedFunctionPointerAttribute");
            return Marshal.GetDelegateForFunctionPointer(addr, typeof (T)) as T;
        }
    }
}