using System;
using Process.NET.Utilities;

namespace Process.NET.Assembly.CallingConventions
{
    /// <summary>
    ///     Static class providing calling convention instances.
    /// </summary>
    public static class CallingConventionSelector
    {
        /// <summary>
        ///     Gets a calling convention object according the given type.
        /// </summary>
        /// <param name="callingConvention">The type of calling convention to get.</param>
        /// <returns>The return value is a singleton of a <see cref="ICallingConvention" /> child.</returns>
        public static ICallingConvention Get(Native.Types.CallingConventions callingConvention)
        {
            switch (callingConvention)
            {
                case Native.Types.CallingConventions.Cdecl:
                    return Singleton<CdeclCallingConvention>.Instance;
                case Native.Types.CallingConventions.Stdcall:
                    return Singleton<StdcallCallingConvention>.Instance;
                case Native.Types.CallingConventions.Fastcall:
                    return Singleton<FastcallCallingConvention>.Instance;
                case Native.Types.CallingConventions.Thiscall:
                    return Singleton<ThiscallCallingConvention>.Instance;
                default:
                    throw new ApplicationException("Unsupported calling convention.");
            }
        }
    }
}