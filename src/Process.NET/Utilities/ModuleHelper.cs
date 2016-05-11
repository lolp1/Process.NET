using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Process.NET.Native;

namespace Process.NET.Utilities
{
    /// <summary>
    ///     Static core class providing tools for manipulating modules and libraries.
    /// </summary>
    public static class ModuleHelper
    {
        /// <summary>
        ///     Retrieves the address of an exported function or variable from the specified dynamic-link library (DLL).
        /// </summary>
        /// <param name="moduleName">The module name (not case-sensitive).</param>
        /// <param name="functionName">The function or variable name, or the function's ordinal value.</param>
        /// <returns>The address of the exported function.</returns>
        public static IntPtr GetProcAddress(string moduleName, string functionName)
        {
            // Get the module
            var module =
                System.Diagnostics.Process.GetCurrentProcess()
                    .Modules.Cast<ProcessModule>()
                    .FirstOrDefault(m => m.ModuleName.ToLower() == moduleName.ToLower());

            // Check whether there is a module loaded with this name
            if (module == null)
                throw new ArgumentException(
                    $"Couldn't get the module {moduleName} because it doesn't exist in the current process.");

            // Get the function address
            var ret = Kernel32.GetProcAddress(module.BaseAddress, functionName);

            // Check whether the function was found
            if (ret != IntPtr.Zero)
                return ret;

            // Else the function was not found, throws an exception
            throw new Win32Exception($"Couldn't get the function address of {functionName}.");
        }

        /// <summary>
        ///     Retrieves the address of an exported function or variable from the specified dynamic-link library (DLL).
        /// </summary>
        /// <param name="module">The <see cref="ProcessModule" /> object corresponding to the module.</param>
        /// <param name="functionName">The function or variable name, or the function's ordinal value.</param>
        /// <returns>If the function succeeds, the return value is the address of the exported function.</returns>
        public static IntPtr GetProcAddress(ProcessModule module, string functionName)
        {
            return GetProcAddress(module.ModuleName, functionName);
        }

        /// <summary>
        ///     Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count.
        /// </summary>
        /// <param name="libraryName">The name of the library to free (not case-sensitive).</param>
        public static void FreeLibrary(string libraryName)
        {
            // Get the module
            var module =
                System.Diagnostics.Process.GetCurrentProcess()
                    .Modules.Cast<ProcessModule>()
                    .FirstOrDefault(m => m.ModuleName.ToLower() == libraryName.ToLower());

            // Check whether there is a library loaded with this name
            if (module == null)
                throw new ArgumentException(
                    $"Couldn't free the library {libraryName} because it doesn't exist in the current process.");

            // Free the library
            if (!Kernel32.FreeLibrary(module.BaseAddress))
                throw new Win32Exception($"Couldn't free the library {libraryName}.");
        }

        /// <summary>
        ///     Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count.
        /// </summary>
        /// <param name="module">The <see cref="ProcessModule" /> object corresponding to the library to free.</param>
        public static void FreeLibrary(ProcessModule module)
        {
            FreeLibrary(module.ModuleName);
        }

        /// <summary>
        ///     Loads the specified module into the address space of the calling process.
        /// </summary>
        /// <param name="libraryPath">
        ///     The name of the module. This can be either a library module (a .dll file) or an executable
        ///     module (an .exe file).
        /// </param>
        /// <returns>A <see cref="ProcessModule" /> corresponding to the loaded library.</returns>
        public static ProcessModule LoadLibrary(string libraryPath)
        {
            // Check whether the file exists
            if (!File.Exists(libraryPath))
                throw new FileNotFoundException(
                    $"Couldn't load the library {libraryPath} because the file doesn't exist.");

            // Load the library
            if (Kernel32.LoadLibrary(libraryPath) == IntPtr.Zero)
                throw new Win32Exception($"Couldn't load the library {libraryPath}.");

            // Enumerate the loaded modules and return the one newly added
            return
                System.Diagnostics.Process.GetCurrentProcess()
                    .Modules.Cast<ProcessModule>()
                    .First(m => m.FileName == libraryPath);
        }
    }
}