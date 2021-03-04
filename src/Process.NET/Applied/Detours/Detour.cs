using System;
using System.Collections.Generic;
using Process.NET.Memory;

namespace Process.NET.Applied.Detours
{
    /// <summary>
    ///     Base class for detour operations and properties. This base class exist
    ///     so that detours can be implemented for different processor architectures
    ///     (such as x86 and x64), operating systems, etc.
    /// </summary>
    /// <seealso cref="T:Process.NET.Applied.IComplexApplied" />
    public abstract class Detour : IComplexApplied
    {
        /// <summary>
        ///     Calls the original function, and returns a return value.
        /// </summary>
        /// <param name="args">
        ///     The arguments to pass. If it is a ' <see langword="void" /> '
        ///     argument list, you MUST pass ' <see langword="null" /> '.
        /// </param>
        /// <returns>
        ///     An object containing the original functions return value.
        /// </returns>
        public object CallOriginal(params object[] args)
        {
            Disable();
            var callOriginal = TargetDelegate?.DynamicInvoke(args);
            Enable();
            return callOriginal;
        }

        /// <summary>
        ///     Disables <see langword="this" /> instance.
        /// </summary>
        public void Disable() => Disable(false);

        /// <summary>
        ///     Removes <see langword="this" /> <see cref="Detour" /> from memory,
        ///     by reverting the modified bytes back to their original values.
        /// </summary>
        public void Disable(bool disableDueToRules)
        {
            if (IgnoreRules & disableDueToRules)
            {
                return;
            }

            DisabledDueToRules = disableDueToRules;
            ProcessMemory.Write(Target, Original.ToArray());
            IsEnabled = false;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing,
        ///     releasing, or resetting unmanaged resources. In this case, it will
        ///     disable the <see cref="Detour64" /> instance and suppress the
        ///     finalizer.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            if (IsEnabled && MustBeDisposed)
            {
                Disable();
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Enables <see langword="this" /> instance.
        /// </summary>
        public void Enable() => Enable(false);

        /// <summary>
        ///     Applies <see langword="this" /> <see cref="Detour" /> to memory.
        ///     (Writes new bytes to memory)
        /// </summary>
        /// <returns>
        /// </returns>
        public void Enable(bool disableDueToRules)
        {
            if (disableDueToRules && DisabledDueToRules)
            {
                DisabledDueToRules = false;
                ProcessMemory.Write(Target, New.ToArray());
                IsEnabled = true;
            }
            else
            {
                if (DisabledDueToRules)
                {
                    return;
                }
                if (IsEnabled)
                {
                    return;
                }

                ProcessMemory.Write(Target, New.ToArray());
                IsEnabled = true;
            }
        }

        /// <summary>
        ///     Get a value indicating if the detour has been disabled due to a the
        ///     specified rules.
        /// </summary>
        public bool DisabledDueToRules { get; set; }

        /// <summary>
        ///     Gets the pointer to be hooked/being hooked.
        /// </summary>
        /// <value>
        ///     The pointer to be hooked/being hooked.
        /// </value>
        public IntPtr HookPointer { get; protected set; }

        /// <summary>
        ///     The name of the detour.
        /// </summary>
        /// <value>
        ///     The name of the detour.
        /// </value>
        public string Identifier { get; protected set; }

        /// <summary>
        ///     Gets a value indicating if the detour should never be disabled by
        ///     the rules logic.
        /// </summary>
        public bool IgnoreRules { get; protected set; }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Detour64" /> is
        ///     disposed.
        /// </summary>
        public bool IsDisposed { get; internal set; }

        /// <summary>
        ///     States if the detour is currently enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Detour64" /> must be
        ///     disposed when the Garbage Collector collects the object.
        /// </summary>
        public bool MustBeDisposed { get; set; } = true;

        /// <summary>
        ///     Gets the new bytes.
        /// </summary>
        /// <value>
        ///     The new bytes.
        /// </value>
        public List<byte> New { get; protected set; }

        /// <summary>
        ///     Gets the original bytes.
        /// </summary>
        /// <value>
        ///     The original bytes.
        /// </value>
        public List<byte> Original { get; protected set; }

        /// <summary>
        ///     <para>
        ///         The reference of the
        ///         <see cref="Process.NET.Applied.Detours.Detour.ProcessMemory" />
        ///     </para>
        ///     <para>object.</para>
        /// </summary>
        public IMemory ProcessMemory { get; protected set; }

        /// <summary>
        ///     Gets the pointer of the target function.
        /// </summary>
        /// <value>
        ///     The pointer of the target function.
        /// </value>
        public IntPtr Target { get; protected set; }

        /// <summary>
        ///     Gets the targeted <see langword="delegate" /> instance.
        /// </summary>
        /// <value>
        ///     The targeted <see langword="delegate" /> instance.
        /// </value>
        public Delegate TargetDelegate { get; protected set; }

        /// <summary>
        ///     Gets or sets the hook <see langword="delegate" /> .
        /// </summary>
        /// <value>
        ///     The hook delegate.
        /// </value>
        protected Delegate HookDelegate { get; set; }

        /// <summary>
        ///     Finalizes an instance of the <see cref="Detour" /> class.
        /// </summary>
        ~Detour()
        {
            if (!IsDisposed)
            {
                Dispose();
            }
        }
    }
}