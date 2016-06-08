using System;
using System.Text;
using Process.NET.Memory;
using Process.NET.Native.Types;

namespace Process.NET
{
    /// <summary>
    ///     Class for memory editing a process.
    /// </summary>
    /// <seealso cref="IMemory" />
    public abstract class ProcessMemory : IMemory
    {
        /// <summary>
        ///     The open handle to the process which contains the memory of interest,
        /// </summary>
        protected readonly SafeMemoryHandle Handle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProcessMemory" /> class.
        /// </summary>
        /// <param name="handle">The open handle to the process which contains the memory of interest.</param>
        protected ProcessMemory(SafeMemoryHandle handle)
        {
            Handle = handle;
        }

        /// <summary>
        ///     Writes a set of bytes to memory.
        /// </summary>
        /// <param name="intPtr">The address where the bytes start in memory.</param>
        /// <param name="length">The length of the byte chunk to read from the memory address.</param>
        /// <returns>
        ///     The byte array section read from memory.
        /// </returns>
        public abstract byte[] Read(IntPtr intPtr, int length);

        /// <summary>
        ///     Reads a string with a specified encoding from memory.
        /// </summary>
        /// <param name="intPtr">The address where the string is read.</param>
        /// <param name="encoding">The encoding used.</param>
        /// <param name="maxLength">
        ///     The number of maximum bytes to read. The string is automatically cropped at this end ('\0'
        ///     char).
        /// </param>
        /// <returns>The string.</returns>
        public string Read(IntPtr intPtr, Encoding encoding, int maxLength)
        {
            var buffer = Read(intPtr, maxLength);
            var ret = encoding.GetString(buffer);
            if (ret.IndexOf('\0') != -1)
                ret = ret.Remove(ret.IndexOf('\0'));
            return ret;
        }

        /// <summary>
        ///     Reads the value of a specified type from memory.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="intPtr">The address where the value is read.</param>
        /// <returns>A value.</returns>
        public abstract T Read<T>(IntPtr intPtr);

        /// <summary>
        ///     Reads an array of a specified type from memory.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="intPtr">The address where the values is read.</param>
        /// <param name="length">The number of cells in the array.</param>
        /// <returns>An array.</returns>
        public T[] Read<T>(IntPtr intPtr, int length)
        {
            var buffer = new T[length];
            for (var i = 0; i < buffer.Length; i++)
                buffer[i] = Read<T>(intPtr);
            return buffer;
        }

        /// <summary>
        ///     Write an array of bytes in the remote process.
        /// </summary>
        /// <param name="intPtr">The address where the array is written.</param>
        /// <param name="bytesToWrite">The array of bytes to write.</param>
        public abstract int Write(IntPtr intPtr, byte[] bytesToWrite);

        /// <summary>
        ///     Writes a string with a specified encoding to memory.
        /// </summary>
        /// <param name="intPtr">The address where the string is written.</param>
        /// <param name="stringToWrite">The text to write.</param>
        /// <param name="encoding">The encoding used.</param>
        public virtual void Write(IntPtr intPtr, string stringToWrite, Encoding encoding)
        {
            if (stringToWrite[stringToWrite.Length - 1] != '\0')
                stringToWrite += '\0';
            var bytes = encoding.GetBytes(stringToWrite);
            Write(intPtr, bytes);
        }

        /// <summary>
        ///     Writes an array of a specified type to memory,
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="intPtr">The address where the values is written.</param>
        /// <param name="values">The array to write.</param>
        public void Write<T>(IntPtr intPtr, T[] values)
        {
            foreach (var value in values)
                Write(intPtr, value);
        }

        /// <summary>
        ///     Writes the values of a specified type to memory.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="intPtr">The address where the value is written.</param>
        /// <param name="value">The value to write.</param>
        public abstract void Write<T>(IntPtr intPtr, T value);
    }
}