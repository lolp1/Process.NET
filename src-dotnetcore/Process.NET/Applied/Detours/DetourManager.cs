using System;
using Process.NET.Extensions;
using Process.NET.Memory;
using Process.NET.Utilities;

namespace Process.NET.Applied.Detours
{
    /// <summary>
    ///     A manager class to handle function detours, and hooks.
    ///     <remarks>All credits to Apoc.</remarks>
    /// </summary>
    public class DetourManager : ComplexAppliedManager<Detour>, IDisposable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DetourManager" /> class.
        /// </summary>
        public DetourManager(IMemory processPlus)
        {
            ProcessPlus = processPlus;
        }

        protected IMemory ProcessPlus { get; set; }

        /// <summary>
        ///     Releases all resources used by the <see cref="DetourManager" /> object.
        /// </summary>
        public void Dispose()
        {
            RemoveAll();
        }

        /// <summary>
        ///     Creates a new Detour.
        /// </summary>
        /// <param name="target">
        ///     The original function to detour. (This delegate should already be registered via
        ///     Magic.RegisterDelegate)
        /// </param>
        /// <param name="newTarget">The new function to be called. (This delegate should NOT be registered!)</param>
        /// <param name="name">The name of the detour.</param>
        /// <param name="ignoreAntiCheatRules"></param>
        /// <returns>
        ///     A <see cref="Detour" /> object containing the required methods to apply, remove, and call the original
        ///     function.
        /// </returns>
        public Detour Create(Delegate target, Delegate newTarget, string name, bool ignoreAntiCheatRules = false)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (newTarget == null)
                throw new ArgumentNullException(nameof(newTarget));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (!target.IsUnmanagedFunctionPointer())
                throw new Exception("The target delegate does not have the proper UnmanagedFunctionPointer attribute!");

            if (!newTarget.IsUnmanagedFunctionPointer())
                throw new Exception(
                    "The new target delegate does not have the proper UnmanagedFunctionPointer attribute!");

            if (InternalItems.ContainsKey(name))
                throw new ArgumentException($"The {name} detour already exists!", nameof(name));

            InternalItems[name] = new Detour(target, newTarget, name, ProcessPlus, ignoreAntiCheatRules);
            return InternalItems[name];
        }

        /// <summary>
        ///     Creates and applies new Detour.
        /// </summary>
        /// <param name="target">
        ///     The original function to detour. (This delegate should already be registered via
        ///     Magic.RegisterDelegate)
        /// </param>
        /// <param name="newTarget">The new function to be called. (This delegate should NOT be registered!)</param>
        /// <param name="name">The name of the detour.</param>
        /// <param name="ignoreAntiCheatRules"></param>
        /// <returns>
        ///     A <see cref="Detour" /> object containing the required methods to apply, remove, and call the original
        ///     function.
        /// </returns>
        public Detour CreateAndApply(Delegate target, Delegate newTarget, string name,
            bool ignoreAntiCheatRules = false)
        {
            Create(target, newTarget, name, ignoreAntiCheatRules);
            InternalItems[name].Enable();
            return InternalItems[name];
        }
    }
}