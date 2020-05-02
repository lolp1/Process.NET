using System;
using System.Diagnostics;
using Process.NET.Utilities;

namespace Process.NET.Extensions
{
    public static class ProcessModuleExtensions
    {
        /// <summary>
        ///     Retrieves the address of an exported function or variable from the specified dynamic-link library (DLL).
        /// </summary>
        /// <param name="module">The <see cref="ProcessModule" /> object corresponding to the module.</param>
        /// <param name="functionName">The function or variable name, or the function's ordinal value.</param>
        /// <returns>If the function succeeds, the return value is the address of the exported function.</returns>
        public static IntPtr GetProcAddress(this ProcessModule module, string functionName)
        {
            return ModuleHelper.GetProcAddress(module.ModuleName, functionName);
        }

        /// <summary>
        ///     Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count.
        /// </summary>
        /// <param name="module">The <see cref="ProcessModule" /> object corresponding to the library to free.</param>
        public static void FreeLibrary(this ProcessModule module)
        {
            ModuleHelper.FreeLibrary(module.ModuleName);
        }

    }
}