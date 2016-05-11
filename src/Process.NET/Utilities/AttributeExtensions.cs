using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Process.NET.Utilities
{
    public static class AttributeExtensions
    {
        public static T GetAttribute<T>(this Type type)
        {
            return
                (T)
                    type.GetCustomAttributes(typeof (T), false).FirstOrDefault();
        }

        public static bool HasAttribute<T>(this Type item)
        {
            return item.GetCustomAttributes(typeof (T), true).Length != 0;
        }

        public static T GetAttributes<T>(this Type type)
        {
            if (HasAttribute<T>(type))
                return
                    (T)
                        type.GetCustomAttributes(typeof (T), false).FirstOrDefault();
            throw new Exception($"No attirubute found for {type}.");
        }

        public static bool IsUnmanagedFunctionPointer(this Delegate d)
        {
            return IsUnmanagedFunctionPointer(d.GetType());
        }

        public static bool IsUnmanagedFunctionPointer(this Type t)
        {
            return HasAttribute<UnmanagedFunctionPointerAttribute>(t);
        }
    }
}