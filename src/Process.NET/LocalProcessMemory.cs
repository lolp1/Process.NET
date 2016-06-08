using System;
using System.Runtime.InteropServices;
using Process.NET.Extensions;
using Process.NET.Memory;
using Process.NET.Native.Types;
using Process.NET.Utilities;

namespace Process.NET
{
    /// <summary>
    ///     Class for memory editing a process.
    /// </summary>
    /// <seealso cref="Process.NET.ProcessMemory" />
    public class LocalProcessMemory : ProcessMemory
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LocalProcessMemory" /> class.
        /// </summary>
        /// <param name="handle">The process.</param>
        public LocalProcessMemory(SafeMemoryHandle handle) : base(handle)
        {
        }

        /// <summary>
        ///     Writes a set of bytes to memory.
        /// </summary>
        /// <param name="intPtr">The address where the bytes start in memory.</param>
        /// <param name="length">The length of the byte chunk to read from the memory address.</param>
        /// <returns>
        ///     The byte array section read from memory.
        /// </returns>
        public override byte[] Read(IntPtr intPtr, int length)
        {
            var readBytes = new byte[length];
            unsafe
            {
                var bytes = (byte*) intPtr;
                for (var i = 0; i < length; i++)
                    readBytes[i] = bytes[i];
            }
            return readBytes;
        }

        /// <summary>
        ///     Reads the value of a specified type from memory.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="intPtr">The address where the value is read.</param>
        /// <returns>A value.</returns>
        public override T Read<T>(IntPtr intPtr)
        {
            return intPtr.Read<T>();
        }

        /// <summary>
        ///     Write an array of bytes in the remote process.
        /// </summary>
        /// <param name="intPtr">The address where the array is written.</param>
        /// <param name="bytesToWrite">The array of bytes to write.</param>
        public override int Write(IntPtr intPtr, byte[] bytesToWrite)
        {
            using (
                new MemoryProtectionOperation(intPtr, bytesToWrite.Length, 0x40))
                unsafe
                {
                    var ptr = (byte*) intPtr;
                    for (var i = 0; i < bytesToWrite.Length; i++)
                        ptr[i] = bytesToWrite[i];
                }
            return bytesToWrite.Length;
        }

        /// <summary>
        ///     Writes the values of a specified type to memory.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="intPtr">The address where the value is written.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="replace">
        ///     If the current value should be replaced. See
        ///     https://msdn.microsoft.com/en-us/library/4ca6d5z7(v=vs.100).aspx for details.
        /// </param>
        public void Write<T>(IntPtr intPtr, T value, bool replace)
        {
            Marshal.StructureToPtr(value, intPtr, replace);
        }

        /// <summary>
        ///     Writes the values of a specified type to memory.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="intPtr">The address where the value is written.</param>
        /// <param name="value">The value to write.</param>
        public override void Write<T>(IntPtr intPtr, T value)
        {
            Write(intPtr, value, false);
        }
    }
}