using System;
using System.Text;
using Process.NET.Native.Types;

namespace Process.NET.Memory
{
    /// <summary>
    ///     Class representing a pointer in the memory of the remote process.
    /// </summary>
    public class MemoryPointer : IEquatable<MemoryPointer>, IPointer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MemoryPointer" /> class.
        /// </summary>
        /// <param name="process">The reference of the <see cref="IProcess"></see></param>
        /// <param name="address">The location where the pointer points in the remote process.</param>
        public MemoryPointer(IProcess process, IntPtr address)
        {
            // Save the parameters
            Process = process;
            BaseAddress = address;
        }

        /// <summary>
        ///     The reference of the <see cref="IMemory" /> object.
        /// </summary>
        public IProcess Process { get; protected set; }

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        public bool Equals(MemoryPointer other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) ||
                   (BaseAddress.Equals(other.BaseAddress) && Process.Equals(other.Process));
        }

        /// <summary>
        ///     The address of the pointer in the remote process.
        /// </summary>
        public IntPtr BaseAddress { get; protected set; }

        /// <summary>
        ///     Gets if the <see cref="MemoryPointer" /> is valid.
        /// </summary>
        public virtual bool IsValid => BaseAddress != IntPtr.Zero;

        public byte[] Read(int offset, int length)
        {
            return Process.Memory.Read(BaseAddress + offset, length);
        }

        /// <summary>
        ///     Reads the value of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="offset">The offset where the value is read from the pointer.</param>
        /// <returns>A value.</returns>
        public T Read<T>(int offset)
        {
            return Process.Memory.Read<T>(BaseAddress + offset);
        }

        /// <summary>
        ///     Reads an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="offset">The offset where the values is read from the pointer.</param>
        /// <param name="count">The number of cells in the array.</param>
        /// <returns>An array.</returns>
        public T[] Read<T>(int offset, int count)
        {
            return Process.Memory.Read<T>(BaseAddress + offset, count);
        }

        public int Write(int offset, byte[] toWrite)
        {
            return Process.Memory.Write(BaseAddress + offset, toWrite);
        }

        /// <summary>
        ///     Reads a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="offset">The offset where the string is read from the pointer.</param>
        /// <param name="encoding">The encoding used.</param>
        /// <param name="maxLength">
        ///     [Optional] The number of maximum bytes to read. The string is automatically cropped at this end
        ///     ('\0' char).
        /// </param>
        /// <returns>The string.</returns>
        public string Read(int offset, Encoding encoding, int maxLength = 512)
        {
            return Process.Memory.Read(BaseAddress + offset, encoding, maxLength);
        }

        /// <summary>
        ///     Writes the values of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="offset">The offset where the value is written from the pointer.</param>
        /// <param name="value">The value to write.</param>
        public void Write<T>(int offset, T value)
        {
            Process.Memory.Write(BaseAddress + offset, value);
        }

        /// <summary>
        ///     Writes an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="offset">The offset where the values is written from the pointer.</param>
        /// <param name="array">The array to write.</param>
        public void Write<T>(int offset, T[] array)
        {
            Process.Memory.Write(BaseAddress + offset, array);
        }

        /// <summary>
        ///     Writes a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="offset">The offset where the string is written from the pointer.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="encoding">The encoding used.</param>
        public void Write(int offset, string text, Encoding encoding)
        {
            Process.Memory.Write(BaseAddress + offset, text, encoding);
        }

        /// <summary>
        ///     Changes the protection of the n next bytes in remote process.
        /// </summary>
        /// <param name="size">The size of the memory to change.</param>
        /// <param name="protection">The new protection to apply.</param>
        /// <param name="mustBeDisposed">The resource will be automatically disposed when the finalizer collects the object.</param>
        /// <returns>A new instance of the <see cref="MemoryProtection" /> class.</returns>
        public MemoryProtection ChangeProtection(SafeMemoryHandle handle, int size,
            MemoryProtectionFlags protection = MemoryProtectionFlags.ExecuteReadWrite, bool mustBeDisposed = true)
        {
            return new MemoryProtection(handle, BaseAddress, size, protection, mustBeDisposed);
        }

        /// <summary>
        ///     Reads the value of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="offset">The offset where the value is read from the pointer.</param>
        /// <returns>A value.</returns>
        public T Read<T>(Enum offset)
        {
            return Read<T>(Convert.ToInt32(offset));
        }

        /// <summary>
        ///     Reads the value of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>A value.</returns>
        public T Read<T>()
        {
            return Read<T>(0);
        }

        /// <summary>
        ///     Reads an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="offset">The offset where the values is read from the pointer.</param>
        /// <param name="count">The number of cells in the array.</param>
        /// <returns>An array.</returns>
        public T[] Read<T>(Enum offset, int count)
        {
            return Read<T>(Convert.ToInt32(offset), count);
        }

        /// <summary>
        ///     Reads a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="offset">The offset where the string is read from the pointer.</param>
        /// <param name="encoding">The encoding used.</param>
        /// <param name="maxLength">
        ///     [Optional] The number of maximum bytes to read. The string is automatically cropped at this end
        ///     ('\0' char).
        /// </param>
        /// <returns>The string.</returns>
        public string Read(Enum offset, Encoding encoding, int maxLength = 512)
        {
            return Read(Convert.ToInt32(offset), encoding, maxLength);
        }

        /// <summary>
        ///     Reads a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="encoding">The encoding used.</param>
        /// <param name="maxLength">
        ///     [Optional] The number of maximum bytes to read. The string is automatically cropped at this end
        ///     ('\0' char).
        /// </param>
        /// <returns>The string.</returns>
        public string Read(Encoding encoding, int maxLength = 512)
        {
            return Read(0, encoding, maxLength);
        }

        /// <summary>
        ///     Reads a string using the encoding UTF8 in the remote process.
        /// </summary>
        /// <param name="offset">The offset where the string is read from the pointer.</param>
        /// <param name="maxLength">
        ///     [Optional] The number of maximum bytes to read. The string is automatically cropped at this end
        ///     ('\0' char).
        /// </param>
        /// <param name="encoding"></param>
        /// <returns>The string.</returns>
        public string Read(int offset, int maxLength, Encoding encoding)
        {
            return Process.Memory.Read(BaseAddress + offset, encoding, maxLength);
        }

        /// <summary>
        ///     Reads a string using the encoding UTF8 in the remote process.
        /// </summary>
        /// <param name="offset">The offset where the string is read from the pointer.</param>
        /// <param name="maxLength">
        ///     [Optional] The number of maximum bytes to read. The string is automatically cropped at this end
        ///     ('\0' char).
        /// </param>
        /// <returns>The string.</returns>
        public string Read(Enum offset, int maxLength = 512)
        {
            return Read(Convert.ToInt32(offset), maxLength, Encoding.UTF8);
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return $"BaseAddress = 0x{BaseAddress.ToInt64():X}";
        }

        /// <summary>
        ///     Writes the values of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="offset">The offset where the value is written from the pointer.</param>
        /// <param name="value">The value to write.</param>
        public void Write<T>(Enum offset, T value)
        {
            Write(Convert.ToInt32(offset), value);
        }

        /// <summary>
        ///     Writes the values of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to write.</param>
        public void Write<T>(T value)
        {
            Write(0, value);
        }

        /// <summary>
        ///     Writes an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="offset">The offset where the values is written from the pointer.</param>
        /// <param name="array">The array to write.</param>
        public void Write<T>(Enum offset, T[] array)
        {
            Write(Convert.ToInt32(offset), array);
        }

        /// <summary>
        ///     Writes an array of a specified type in the remote process.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="array">The array to write.</param>
        public void Write<T>(T[] array)
        {
            Write(0, array);
        }

        /// <summary>
        ///     Writes a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="offset">The offset where the string is written from the pointer.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="encoding">The encoding used.</param>
        public void Write(Enum offset, string text, Encoding encoding)
        {
            Write(Convert.ToInt32(offset), text, encoding);
        }

        /// <summary>
        ///     Writes a string with a specified encoding in the remote process.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <param name="encoding">The encoding used.</param>
        public void Write(string text, Encoding encoding)
        {
            Write(0, text, encoding);
        }

        /// <summary>
        ///     Writes a string using the encoding UTF8 in the remote process.
        /// </summary>
        /// <param name="offset">The offset where the string is written from the pointer.</param>
        /// <param name="text">The text to write.</param>
        public void Write(int offset, string text)
        {
            Process.Memory.Write(BaseAddress + offset, text);
        }

        /// <summary>
        ///     Writes a string using the encoding UTF8 in the remote process.
        /// </summary>
        /// <param name="offset">The offset where the string is written from the pointer.</param>
        /// <param name="text">The text to write.</param>
        public void Write(Enum offset, string text)
        {
            Write(Convert.ToInt32(offset), text);
        }

        /// <summary>
        ///     Writes a string using the encoding UTF8 in the remote process.
        /// </summary>
        /// <param name="text">The text to write.</param>
        public void Write(string text)
        {
            Write(0, text);
        }

        /// <summary>
        ///     Determines whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((MemoryPointer) obj);
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        public override int GetHashCode()
        {
            return BaseAddress.GetHashCode() ^ Process.GetHashCode();
        }

        public static bool operator ==(MemoryPointer left, MemoryPointer right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MemoryPointer left, MemoryPointer right)
        {
            return !Equals(left, right);
        }
    }
}