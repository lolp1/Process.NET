using System;
using System.Text;

namespace Process.NET.Memory
{
    public interface IMemory
    {
        /// <summary>
        ///     Writes a set of bytes to memory.
        /// </summary>
        /// <param name="intPtr">The address where the bytes start in memory.</param>
        /// <param name="length">The slength of the byte chunk to read from the memory address.</param>
        /// <returns>
        ///     The byte array section read from memory.
        /// </returns>
        byte[] Read(IntPtr intPtr, int length);

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
        string Read(IntPtr intPtr, Encoding encoding, int maxLength);

        /// <summary>
        ///     Reads the value of a specified type from memory.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="intPtr">The address where the value is read.</param>
        /// <returns>A value.</returns>
        T Read<T>(IntPtr intPtr);

        /// <summary>
        ///     Reads an array of a specified type from memory.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="intPtr">The address where the values is read.</param>
        /// <param name="length">The number of cells in the array.</param>
        /// <returns>An array.</returns>
        T[] Read<T>(IntPtr intPtr, int length);

        /// <summary>
        ///     Write an array of bytes in the remote process.
        /// </summary>
        /// <param name="intPtr">The address where the array is written.</param>
        /// <param name="bytesToWrite">The array of bytes to write.</param>
        int Write(IntPtr intPtr, byte[] bytesToWrite);

        /// <summary>
        ///     Writes a string with a specified encoding to memory.
        /// </summary>
        /// <param name="intPtr">The address where the string is written.</param>
        /// <param name="stringToWrite">The text to write.</param>
        /// <param name="encoding">The encoding used.</param>
        void Write(IntPtr intPtr, string stringToWrite, Encoding encoding);

        /// <summary>
        ///     Writes an array of a specified type to memory,
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="intPtr">The address where the values is written.</param>
        /// <param name="values">The array to write.</param>
        void Write<T>(IntPtr intPtr, T[] values);

        /// <summary>
        ///     Writes the values of a specified type to memory.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="intPtr">The address where the value is written.</param>
        /// <param name="value">The value to write.</param>
        void Write<T>(IntPtr intPtr, T value);
    }
}