﻿using System;
using System.Runtime.InteropServices;

namespace Process.NET.Memory
{
    /// <summary>
    ///     Class representing a block of memory allocated in the local process.
    /// </summary>
    public class LocalUnmanagedMemory : IDisposable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LocalUnmanagedMemory" /> class, allocating a block of memory in the
        ///     local process.
        /// </summary>
        /// <param name="size">The size to allocate.</param>
        public LocalUnmanagedMemory(int size)
        {
            // Allocate the memory
            Size = size;
            Address = Marshal.AllocHGlobal(Size);
        }

        /// <summary>
        ///     The address where the data is allocated.
        /// </summary>
        public IntPtr Address { get; private set; }

        /// <summary>
        ///     The size of the allocated memory.
        /// </summary>
        public int Size { get; }

        /// <summary>
        ///     Releases the memory held by the <see cref="LocalUnmanagedMemory" /> object.
        /// </summary>
        public virtual void Dispose()
        {
            // Free the allocated memory
            Marshal.FreeHGlobal(Address);
            // Remove the pointer
            Address = IntPtr.Zero;
            // Avoid the finalizer
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Frees resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~LocalUnmanagedMemory()
        {
            Dispose();
        }

        /// <summary>
        ///     Reads data from the unmanaged block of memory.
        /// </summary>
        /// <typeparam name="T">The type of data to return.</typeparam>
        /// <returns>The return value is the block of memory casted in the specified type.</returns>
        public T Read<T>()
        {
            // Marshal data from the block of memory to a new allocated managed object
            return (T) Marshal.PtrToStructure(Address, typeof (T));
        }

        /// <summary>
        ///     Reads an array of bytes from the unmanaged block of memory.
        /// </summary>
        /// <returns>The return value is the block of memory.</returns>
        public byte[] Read()
        {
            // Allocate an array to store data
            var bytes = new byte[Size];
            // Copy the block of memory to the array
            Marshal.Copy(Address, bytes, 0, Size);
            // Return the array
            return bytes;
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return $"Size = {Size:X}";
        }

        /// <summary>
        ///     Writes an array of bytes to the unmanaged block of memory.
        /// </summary>
        /// <param name="byteArray">The array of bytes to write.</param>
        public void Write(byte[] byteArray)
        {
            // Copy the array of bytes into the block of memory
            Marshal.Copy(byteArray, 0, Address, Size);
        }

        /// <summary>
        ///     Write data to the unmanaged block of memory.
        /// </summary>
        /// <typeparam name="T">The type of data to write.</typeparam>
        /// <param name="data">The data to write.</param>
        public void Write<T>(T data)
        {
            // Marshal data from the managed object to the block of memory
            Marshal.StructureToPtr(data, Address, false);
        }
    }
}