using System;
using System.Collections.Generic;
using Process.NET.Extensions;
using Process.NET.Memory;
using Process.NET.Utilities;

namespace Process.NET.Applied.Detours
{
    /// <summary>
    ///     A manager class to handle function detours, and hooks.
    ///     <remarks>All credits to the GreyMagic library written by Apoc @ www.ownedcore.com</remarks>
    /// </summary>
    public class Detour : IComplexApplied
    {
        /// <summary>
        ///     This var is not used within the detour itself. It is only here
        ///     to keep a reference, to avoid the GC from collecting the delegate instance!
        /// </summary>
        // ReSharper disable once NotAccessedField.Local
        private readonly Delegate _hookDelegate;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Detour" /> class.
        /// </summary>
        /// <param name="target">The target delegate.</param>
        /// <param name="hook">The hook delegate.</param>
        /// <param name="identifier"></param>
        /// <param name="memory">The <see cref="MemoryPlus" /> instance.</param>
        /// <param name="ignoreRules"></param>
        public Detour(Delegate target, Delegate hook, string identifier, IMemory memory,
            bool ignoreRules = false)
        {
            ProcessMemory = memory;
            Identifier = identifier;
            IgnoreRules = ignoreRules;

            TargetDelegate = target;
            Target = target.ToFunctionPtr();

            _hookDelegate = hook;
            HookPointer = hook.ToFunctionPtr(); //target

            //Store the original bytes
            Original = new List<byte>();
            Original.AddRange(memory.Read(Target, 6));

            //Setup the detour bytes
            New = new List<byte> {0x68};
            var tmp = BitConverter.GetBytes(HookPointer.ToInt32());
            New.AddRange(tmp);
            New.Add(0xC3);
        }

        /// <summary>
        ///     The reference of the <see cref="ProcessMemory" /> object.
        /// </summary>
        private IMemory ProcessMemory { get; }

        /// <summary>
        ///     Gets the pointer to be hooked/being hooked.
        /// </summary>
        /// <value>The pointer to be hooked/being hooked.</value>
        public IntPtr HookPointer { get; }

        /// <summary>
        ///     Gets the new bytes.
        /// </summary>
        /// <value>The new bytes.</value>
        public List<byte> New { get; }

        /// <summary>
        ///     Gets the original bytes.
        /// </summary>
        /// <value>The original bytes.</value>
        public List<byte> Original { get; }

        /// <summary>
        ///     Gets the pointer of the target function.
        /// </summary>
        /// <value>The pointer of the target function.</value>
        public IntPtr Target { get; }

        /// <summary>
        ///     Gets the targeted delegate instance.
        /// </summary>
        /// <value>The targeted delegate instance.</value>
        public Delegate TargetDelegate { get; }

        /// <summary>
        ///     Get a value indicating if the detour has been disabled due to a running AntiCheat scan
        /// </summary>
        public bool DisabledDueToRules { get; set; }

        /// <summary>
        ///     Geta s value indicating if the detour should never be disabled by the AntiCheat scan logic
        /// </summary>
        public bool IgnoreRules { get; }

        /// <summary>
        ///     States if the detour is currently enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        ///     The name of the detour.
        /// </summary>
        /// <value>The name of the detour.</value>
        public string Identifier { get; }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Detour" /> is disposed.
        /// </summary>
        public bool IsDisposed { get; internal set; }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Detour" /> must be disposed when the Garbage Collector collects the
        ///     object.
        /// </summary>
        public bool MustBeDisposed { get; set; } = true;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. In this
        ///     case, it will disable the <see cref="Detour" /> instance and suppress the finalizer.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;
            if (IsEnabled)
                Disable();
            GC.SuppressFinalize(this);
        }

        public void Enable()
        {
            Enable(false);
        }

        public void Disable()
        {
            Disable(false);
        }

        /// <summary>
        ///     Removes this Detour from memory. (Reverts the bytes back to their originals.)
        /// </summary>
        /// <returns></returns>
        public void Disable(bool disableDueToRules)
        {
            if (IgnoreRules && disableDueToRules)
                return;

            DisabledDueToRules = disableDueToRules;

            ProcessMemory.Write(Target, Original.ToArray());
            IsEnabled = false;
        }

        /// <summary>
        ///     Applies this Detour to memory. (Writes new bytes to memory)
        /// </summary>
        /// <returns></returns>
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
                    return;

                if (IsEnabled)
                    return;

                ProcessMemory.Write(Target, New.ToArray());
                IsEnabled = true;
            }
        }

        ~Detour()
        {
            if (MustBeDisposed)
                Dispose();
        }

        /// <summary>
        ///     Calls the original function, and returns a return value.
        /// </summary>
        /// <param name="args">
        ///     The arguments to pass. If it is a 'void' argument list,
        ///     you MUST pass 'null'.
        /// </param>
        /// <returns>An object containing the original functions return value.</returns>
        public object CallOriginal(params object[] args)
        {
            Disable();
            var ret = TargetDelegate.DynamicInvoke(args);
            Enable();
            return ret;
        }
    }
}