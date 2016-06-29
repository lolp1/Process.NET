using System;
using Process.NET.Marshaling;
using Process.NET.Native.Types;
using Process.NET.Utilities;

namespace Process.NET.Memory
{
    /// <summary>
    ///     Class for memory editing a process.
    /// </summary>
    /// <seealso cref="ProcessMemory" />
    public class ExternalProcessMemory : ProcessMemory
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExternalProcessMemory" /> class.
        /// </summary>
        /// <param name="handle">The open handle to the process which contains the memory of interest.</param>
        public ExternalProcessMemory(SafeMemoryHandle handle) : base(handle)
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
            return MemoryHelper.ReadBytes(Handle, intPtr, length);
        }

        /// <summary>
        ///     Reads the value of a specified type from memory.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="intPtr">The address where the value is read.</param>
        /// <returns>A value.</returns>
        public override T Read<T>(IntPtr intPtr)
        {
            return MarshalType<T>.ByteArrayToObject(Read(intPtr, MarshalType<T>.Size));
        }

        /// <summary>
        ///     Writes the values of a specified type to memory.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="intPtr">The address where the value is written.</param>
        /// <param name="value">The value to write.</param>
        public override void Write<T>(IntPtr intPtr, T value)
        {
            Write(intPtr, MarshalType<T>.ObjectToByteArray(value));
        }

        /// <summary>
        ///     Write an array of bytes in the remote process.
        /// </summary>
        /// <param name="intPtr">The address where the array is written.</param>
        /// <param name="bytesToWrite">The array of bytes to write.</param>
        public override int Write(IntPtr intPtr, byte[] bytesToWrite)
        {
            using (new MemoryProtection(Handle, intPtr,
                MarshalType<byte>.Size*bytesToWrite.Length))
                MemoryHelper.WriteBytes(Handle, intPtr, bytesToWrite);
            return bytesToWrite.Length;
        }
    }
}