using System;
using System.Runtime.InteropServices;
using System.Text;
using Process.NET.Memory;

// ReSharper disable All

namespace Process.NET.Marshaling
{
    /// <summary>
    ///     Static class providing tools for extracting information related to types.
    /// </summary>
    /// <typeparam name="T">Type to analyse.</typeparam>
    public static class MarshalType<T>
    {
        /// <summary>
        ///     Initializes static information related to the specified type.
        /// </summary>
        static MarshalType()
        {
            // Gather information related to the provided type
            IsIntPtr = typeof (T) == typeof (IntPtr);
            RealType = typeof (T);
            Size = TypeCode == TypeCode.Boolean ? 1 : Marshal.SizeOf(RealType);
            TypeCode = Type.GetTypeCode(RealType);
            // Check if the type can be stored in registers
            if (IntPtr.Size == 8)
                CanBeStoredInRegisters = Test();
            else
                CanBeStoredInRegisters =
                    IsIntPtr ||
                    TypeCode == TypeCode.Boolean ||
                    TypeCode == TypeCode.Byte ||
                    TypeCode == TypeCode.Char ||
                    TypeCode == TypeCode.Int16 ||
                    TypeCode == TypeCode.Int32 ||
                    TypeCode == TypeCode.Int64 ||
                    TypeCode == TypeCode.SByte ||
                    TypeCode == TypeCode.Single ||
                    TypeCode == TypeCode.UInt16 ||
                    TypeCode == TypeCode.UInt32;
        }

        /// <summary>
        ///     Gets if the type can be stored in a registers (for example ACX, ECX, ...).
        /// </summary>
        public static bool CanBeStoredInRegisters { get; }

        /// <summary>
        ///     State if the type is <see cref="IntPtr" />.
        /// </summary>
        public static bool IsIntPtr { get; }

        /// <summary>
        ///     The real type.
        /// </summary>
        public static Type RealType { get; }

        /// <summary>
        ///     The size of the type.
        /// </summary>
        public static int Size { get; }

        /// <summary>
        ///     The typecode of the type.
        /// </summary>
        public static TypeCode TypeCode { get; }

        private static bool Test()
        {
            return IsIntPtr ||
                   TypeCode == TypeCode.Int64 ||
                   TypeCode == TypeCode.UInt64 ||
                   TypeCode == TypeCode.Boolean ||
                   TypeCode == TypeCode.Byte ||
                   TypeCode == TypeCode.Char ||
                   TypeCode == TypeCode.Int16 ||
                   TypeCode == TypeCode.Int32 ||
                   TypeCode == TypeCode.Int64 ||
                   TypeCode == TypeCode.SByte ||
                   TypeCode == TypeCode.Single ||
                   TypeCode == TypeCode.UInt16 ||
                   TypeCode == TypeCode.UInt32;
        }

        /// <summary>
        ///     Marshals a managed object to an array of bytes.
        /// </summary>
        /// <param name="obj">The object to marshal.</param>
        /// <returns>A array of bytes corresponding to the managed object.</returns>
        public static byte[] ObjectToByteArray(T obj)
        {
            // We'll tried to avoid marshalling as it really slows the process
            // First, check if the type can be converted without marhsalling
            switch (TypeCode)
            {
                case TypeCode.Object:
                    if (IsIntPtr)
                        switch (Size)
                        {
                            case 4:
                                return BitConverter.GetBytes(((IntPtr) (object) obj).ToInt32());
                            case 8:
                                return BitConverter.GetBytes(((IntPtr) (object) obj).ToInt64());
                        }
                    break;
                case TypeCode.Boolean:
                    return BitConverter.GetBytes((bool) (object) obj);
                case TypeCode.Char:
                    return Encoding.UTF8.GetBytes(new[] {(char) (object) obj});
                case TypeCode.Double:
                    return BitConverter.GetBytes((double) (object) obj);
                case TypeCode.Int16:
                    return BitConverter.GetBytes((short) (object) obj);
                case TypeCode.Int32:
                    return BitConverter.GetBytes((int) (object) obj);
                case TypeCode.Int64:
                    return BitConverter.GetBytes((long) (object) obj);
                case TypeCode.Single:
                    return BitConverter.GetBytes((float) (object) obj);
                case TypeCode.String:
                    throw new InvalidCastException("This method doesn't support string conversion.");
                case TypeCode.UInt16:
                    return BitConverter.GetBytes((ushort) (object) obj);
                case TypeCode.UInt32:
                    return BitConverter.GetBytes((uint) (object) obj);
                case TypeCode.UInt64:
                    return BitConverter.GetBytes((ulong) (object) obj);
            }
            // Check if it's not a common type
            // Allocate a block of unmanaged memory
            using (var unmanaged = new LocalUnmanagedMemory(Size))
            {
                // Write the object inside the unmanaged memory
                unmanaged.Write(obj);
                // Return the content of the block of unmanaged memory
                return unmanaged.Read();
            }
        }

        /// <summary>
        ///     Marshals an array of byte to a managed object.
        /// </summary>
        /// <param name="byteArray">The array of bytes corresponding to a managed object.</param>
        /// <returns>A managed object.</returns>
        public static T ByteArrayToObject(byte[] byteArray)
        {
            // We'll tried to avoid marshalling as it really slows the process
            // First, check if the type can be converted without marhsalling
            switch (TypeCode)
            {
                case TypeCode.Object:
                    if (IsIntPtr)
                        switch (byteArray.Length)
                        {
                            case 1:
                                return
                                    (T)
                                        (object)
                                            new IntPtr(BitConverter.ToInt32(new byte[] {byteArray[0], 0x0, 0x0, 0x0}, 0));
                            case 2:
                                return
                                    (T)
                                        (object)
                                            new IntPtr(
                                                BitConverter.ToInt32(new byte[] {byteArray[0], byteArray[1], 0x0, 0x0},
                                                    0));
                            case 4:
                                return (T) (object) new IntPtr(BitConverter.ToInt32(byteArray, 0));
                            case 8:
                                return (T) (object) new IntPtr(BitConverter.ToInt64(byteArray, 0));
                        }
                    break;
                case TypeCode.Boolean:
                    return (T) (object) BitConverter.ToBoolean(byteArray, 0);
                case TypeCode.Byte:
                    return (T) (object) byteArray[0];
                case TypeCode.Char:
                    return (T) (object) Encoding.UTF8.GetChars(byteArray)[0]; //BitConverter.ToChar(byteArray, 0);
                case TypeCode.Double:
                    return (T) (object) BitConverter.ToDouble(byteArray, 0);
                case TypeCode.Int16:
                    return (T) (object) BitConverter.ToInt16(byteArray, 0);
                case TypeCode.Int32:
                    return (T) (object) BitConverter.ToInt32(byteArray, 0);
                case TypeCode.Int64:
                    return (T) (object) BitConverter.ToInt64(byteArray, 0);
                case TypeCode.Single:
                    return (T) (object) BitConverter.ToSingle(byteArray, 0);
                case TypeCode.String:
                    throw new InvalidCastException("This method doesn't support string conversion.");
                case TypeCode.UInt16:
                    return (T) (object) BitConverter.ToUInt16(byteArray, 0);
                case TypeCode.UInt32:
                    return (T) (object) BitConverter.ToUInt32(byteArray, 0);
                case TypeCode.UInt64:
                    return (T) (object) BitConverter.ToUInt64(byteArray, 0);
            }
            // Check if it's not a common type
            // Allocate a block of unmanaged memory
            using (var unmanaged = new LocalUnmanagedMemory(Size))
            {
                // Write the array of bytes inside the unmanaged memory
                unmanaged.Write(byteArray);
                // Return a managed object created from the block of unmanaged memory
                return unmanaged.Read<T>();
            }
        }

        /// <summary>
        ///     Converts a pointer to a given type. This function converts the value of the pointer or the pointed value,
        ///     according if the data type is primitive or reference.
        /// </summary>
        /// <param name="memorySharp">The concerned process.</param>
        /// <param name="pointer">The pointer to convert.</param>
        /// <returns>The return value is the pointer converted to the given data type.</returns>
        public static T PtrToObject(IProcess memorySharp, IntPtr pointer)
        {
            return ByteArrayToObject(CanBeStoredInRegisters
                ? BitConverter.GetBytes(pointer.ToInt64())
                : memorySharp.Memory.Read<byte>(pointer, Size));
        }
    }
}