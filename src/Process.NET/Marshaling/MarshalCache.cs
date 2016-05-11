using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Process.NET.Marshaling
{
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    public static class MarshalCache<T>
    {
        public unsafe delegate void* GetUnsafePtrDelegate(ref T value);

        /// <summary> The size of the Type </summary>
        public static int Size;

        /// <summary> The real, underlying type. </summary>
        public static Type RealType;

        /// <summary> The type code </summary>
        public static TypeCode TypeCode;

        /// <summary> True if this type requires the Marshaler to map variables. (No direct pointer dereferencing) </summary>
        public static bool TypeRequiresMarshal;

        public static readonly GetUnsafePtrDelegate GetUnsafePtr;

        static MarshalCache()
        {
            TypeCode = Type.GetTypeCode(typeof (T));

            // Bools = 1 char.
            if (typeof (T) == typeof (bool))
            {
                Size = 1;
                RealType = typeof (T);
            }
            else if (typeof (T).IsEnum)
            {
                var underlying = typeof (T).GetEnumUnderlyingType();
                Size = GetSizeOf(underlying);
                RealType = underlying;
                TypeCode = Type.GetTypeCode(underlying);
            }
            else
            {
                Size = GetSizeOf(typeof (T));
                RealType = typeof (T);
            }

            // Basically, if any members of the type have a MarshalAs attrib, then we can't just pointer deref. :(
            // This literally means any kind of MarshalAs. Strings, arrays, custom type sizes, etc.
            // Ideally, we want to avoid the Marshaler as much as possible. It causes a lot of overhead, and for a memory reading
            // lib where we need the best speed possible, we do things manually when possible!
            TypeRequiresMarshal = RequiresMarshal(RealType);
            //Debug.WriteLine("Type " + typeof(T).Name + " requires marshaling: " + TypeRequiresMarshal);

            // Generate a method to get the address of a generic type. We'll be using this for RtlMoveMemory later for much faster structure reads.
            var method = new DynamicMethod($"GetPinnedPtr<{typeof (T).FullName.Replace(".", "<>")}>",
                typeof (void*),
                new[] {typeof (T).MakeByRefType()},
                typeof (MarshalCache<>).Module);
            var generator = method.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Conv_U);
            generator.Emit(OpCodes.Ret);
            GetUnsafePtr = (GetUnsafePtrDelegate) method.CreateDelegate(typeof (GetUnsafePtrDelegate));
        }

        private static int GetSizeOf(Type t)
        {
            try
            {
                // Note: This is in a try/catch for a reason.

                // A structure doesn't have to be marked as generic, to have generic types INSIDE of it.
                // Marshal.SizeOf will toss an exception when it can't find a size due to a generic type inside it.
                // Also... this just makes sure we can handle any other shenanigans the marshaler does.
                return Marshal.SizeOf(t);
            }
            catch
            {
                // So, chances are, we're using generic sub-types.
                // This is a good, and bad thing.
                // Good for STL implementations, bad for most everything else.
                // But for the sake of completeness, lets make this work.

                var totalSize = 0;

                foreach (var field in t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    // Check if its a fixed-size-buffer. Eg; fixed byte Pad[50];
                    var attr = field.GetCustomAttributes(typeof (FixedBufferAttribute), false);
                    if (attr.Length > 0)
                    {
                        var fba = attr[0] as FixedBufferAttribute;
                        totalSize += GetSizeOf(fba.ElementType)*fba.Length;
                    }

                    // Recursive. We want to allow ourselves to dive back into this function if we need to!
                    totalSize += GetSizeOf(field.FieldType);
                }
                return totalSize;
            }
        }

        private static bool RequiresMarshal(Type t)
        {
            foreach (var fieldInfo in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var requires = fieldInfo.GetCustomAttributes(typeof (MarshalAsAttribute), true).Any();

                if (requires)
                {
                    Debug.WriteLine(fieldInfo.FieldType.Name + " requires marshaling.");
                    return true;
                }

                // Nope
                if (t == typeof (IntPtr) || t == typeof (string))
                    continue;

                // If it's a custom object, then check it separately for marshaling requirements.
                if (Type.GetTypeCode(t) == TypeCode.Object)
                    requires |= RequiresMarshal(fieldInfo.FieldType);

                // if anything requires a marshal, period, no matter where/what it is.
                // just return true. Hop out of this func as early as possible.
                if (requires)
                {
                    Debug.WriteLine(fieldInfo.FieldType.Name + " requires marshaling.");
                    return true;
                }
            }
            return false;
        }
    }
}