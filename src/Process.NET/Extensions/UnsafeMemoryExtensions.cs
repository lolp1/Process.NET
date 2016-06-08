using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Process.NET.Marshaling;
using Process.NET.Native;

namespace Process.NET.Extensions
{
    public static class UnsafeMemoryExtensions
    {

        /// <summary>
        /// Gets an address from a vtable index. Since it uses index * IntPtr, it should work for both x64 and x32. 
        /// </summary>
        /// <param name="intPtr">The int PTR.</param>
        /// <param name="functionIndex">Index of the function.</param>
        /// <returns>IntPtr.</returns>
        public static IntPtr GetVtableIntPtr(this IntPtr intPtr, int functionIndex)
        {
            var vftable = intPtr.Read<IntPtr>();
            return (vftable + functionIndex * IntPtr.Size).Read<IntPtr>();
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

        public static unsafe T Read<T>(this IntPtr address)
        {
            try
            {
                // TODO: Optimize this more. The boxing/unboxing required tends to slow this down.
                // It may be worth it to simply use memcpy to avoid it, but I doubt thats going to give any noticeable increase in speed.
                if (address == IntPtr.Zero)
                    throw new InvalidOperationException("Cannot retrieve a value at address 0");

                object ptrToStructure;
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (MarshalCache<T>.TypeCode)
                {
                    case TypeCode.Object:

                        if (MarshalCache<T>.RealType == typeof(IntPtr))
                            return (T)(object)*(IntPtr*)address;

                        // If the type doesn't require an explicit Marshal call, then ignore it and memcpy the thing.
                        if (!MarshalCache<T>.TypeRequiresMarshal)
                        {
                            var o = default(T);
                            var ptr = MarshalCache<T>.GetUnsafePtr(ref o);

                            Kernel32.MoveMemory(ptr, (void*)address, MarshalCache<T>.Size);

                            return o;
                        }

                        // All System.Object's require marshaling!
                        ptrToStructure = Marshal.PtrToStructure(address, typeof(T));
                        break;
                    case TypeCode.Boolean:
                        ptrToStructure = *(byte*)address != 0;
                        break;
                    case TypeCode.Char:
                        ptrToStructure = *(char*)address;
                        break;
                    case TypeCode.SByte:
                        ptrToStructure = *(sbyte*)address;
                        break;
                    case TypeCode.Byte:
                        ptrToStructure = *(byte*)address;
                        break;
                    case TypeCode.Int16:
                        ptrToStructure = *(short*)address;
                        break;
                    case TypeCode.UInt16:
                        ptrToStructure = *(ushort*)address;
                        break;
                    case TypeCode.Int32:
                        ptrToStructure = *(int*)address;
                        break;
                    case TypeCode.UInt32:
                        ptrToStructure = *(uint*)address;
                        break;
                    case TypeCode.Int64:
                        ptrToStructure = *(long*)address;
                        break;
                    case TypeCode.UInt64:
                        ptrToStructure = *(ulong*)address;
                        break;
                    case TypeCode.Single:
                        ptrToStructure = *(float*)address;
                        break;
                    case TypeCode.Double:
                        ptrToStructure = *(double*)address;
                        break;
                    case TypeCode.Decimal:
                        // Probably safe to remove this. I'm unaware of anything that actually uses "decimal" that would require memory reading...
                        ptrToStructure = *(decimal*)address;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return (T)ptrToStructure;
            }
            catch (AccessViolationException ex)
            {
                Trace.WriteLine("Access Violation on " + address + " with type " + typeof(T).Name + Environment.NewLine +
                                ex);
                return default(T);
            }
        }
    }
}