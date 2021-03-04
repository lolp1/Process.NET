using System;
using System.Collections.Concurrent;
using Process.NET.Extensions;
using Process.NET.Memory;
using Process.NET.Native.Types;

namespace Process.NET.Applied.Detours
{
    /// <summary>
    ///     A manager class to handle function detours, and hooks.
    /// </summary>
    public class DetourManager : ComplexAppliedManager<Detour>, IDisposable
    {
        private bool _disposed;

        /// <summary>
        ///     <para>
        ///         Initializes a new instance of the <see cref="DetourManager" />
        ///     </para>
        ///     <para>class.</para>
        /// </summary>
        public DetourManager(IMemory processPlus) => ProcessPlus = processPlus;

        /// <summary>
        ///     Creates a new <see cref="Detour" /> .
        /// </summary>
        /// <param name="target">
        ///     <para>
        ///         The original function to detour. (This <see langword="delegate" />
        ///     </para>
        ///     <para>
        ///         should already be registered via Magic.RegisterDelegate)
        ///     </para>
        /// </param>
        /// <param name="newTarget">
        ///     <para>
        ///         The new function to be called. (This <see langword="delegate" />
        ///     </para>
        ///     <para>should NOT be registered!)</para>
        /// </param>
        /// <param name="name">The name of the detour.</param>
        /// <param name="ignoreRules"></param>
        /// <returns>
        ///     A <see cref="Detour64" /> object containing the required methods to
        ///     apply, remove, and call the original function.
        /// </returns>
        public Detour Create(Delegate target, Delegate newTarget, string name, bool ignoreRules = false)
        {
            if ((object) target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if ((object) newTarget == null)
            {
                throw new ArgumentNullException(nameof(newTarget));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (!target.IsUnmanagedFunctionPointer())
            {
                throw new InvalidOperationException(
                    @"The target delegate does not have the proper UnmanagedFunctionPointer attribute.");
            }

            if (!newTarget.IsUnmanagedFunctionPointer())
            {
                throw new InvalidOperationException(
                    "The new target delegate does not have the proper UnmanagedFunctionPointer attribute!");
            }

            if (InternalItems.ContainsKey(name))
            {
                Remove(name);
            }

            var internalItem = ProcessorArchitecture == ProcessorArchitecture.X86
                ? new Detour32(target, newTarget, name, ProcessPlus, ignoreRules) as Detour
                : new Detour64(target, newTarget, name, ProcessPlus, ignoreRules);

            Add(internalItem);
            return InternalItems[name];
        }

        /// <summary>
        ///     Creates and applies new <see cref="Detour" /> .
        /// </summary>
        /// <param name="target">
        ///     <para>
        ///         The original function to detour. (This <see langword="delegate" />
        ///     </para>
        ///     <para>
        ///         should already be registered via Magic.RegisterDelegate)
        ///     </para>
        /// </param>
        /// <param name="newTarget">
        ///     <para>
        ///         The new function to be called. (This <see langword="delegate" />
        ///     </para>
        ///     <para>should NOT be registered!)</para>
        /// </param>
        /// <param name="name">
        ///     The n <paramref name="name" /> of the detour.
        /// </param>
        /// <param name="ignoreRules"></param>
        /// <returns>
        ///     A <see cref="Detour64" /> object containing the required methods to
        ///     apply, remove, and call the original function.
        /// </returns>
        public Detour CreateAndApply(Delegate target, Delegate newTarget, string name, bool ignoreRules = false)
        {
            if (InternalItems.ContainsKey(name))
            {
                Remove(name);
            }

            var createAndApply = Create(target, newTarget, name, ignoreRules);

            if (createAndApply == null)
            {
                throw new ArgumentNullException(nameof(createAndApply));
            }

            Enable(createAndApply, ignoreRules);
            return createAndApply;
        }

        /// <summary>
        ///     <para>
        ///         Disables <see cref="Process.NET.Applied.AppliedManager`1" />
        ///     </para>
        ///     <para>instance.</para>
        /// </summary>
        /// <param name="detour">The item.</param>
        /// <exception cref="NotImplementedException" />
        public override void Disable(Detour detour)
        {
            if (detour == null)
            {
                throw new ArgumentNullException(nameof(detour));
            }

            if (detour.IsEnabled)
            {
                detour.Disable(false);
            }
        }

        /// <summary>
        ///     <para>
        ///         Disables <see cref="Process.NET.Applied.AppliedManager`1" />
        ///     </para>
        ///     <para>instance.</para>
        /// </summary>
        /// <param name="name">The <paramref name="name" /> .</param>
        /// <exception cref="ArgumentException">
        ///     Value cannot be <see langword="null" /> or empty. -
        ///     <paramref name="name" />
        /// </exception>
        public override void Disable(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            }

            if (InternalItems.TryGetValue(name, out Detour detour))
            {
                Disable(detour);
            }
        }

        /// <summary>
        ///     Disables <see langword="this" /> instance.
        /// </summary>
        /// <param name="detour">The item.</param>
        /// <param name="dueToRules">
        ///     if set to <c>&lt;see langword="true"</c> [due to rules].
        /// </param>
        /// <exception cref="ArgumentNullException">item</exception>
        public override void Disable(Detour detour, bool dueToRules)
        {
            if (detour == null)
            {
                throw new ArgumentNullException(nameof(detour));
            }

            if (detour.IsEnabled)
            {
                detour.Disable(dueToRules);
            }
        }

        /// <summary>
        ///     Disables <see langword="this" /> instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="dueToRules">
        ///     if set to <c>&lt;see langword="true"</c> [due to rules].
        /// </param>
        public override void Disable(string name, bool dueToRules)
        {
            if (InternalItems.TryGetValue(name, out Detour detour))
            {
                Disable(detour, dueToRules);
            }
        }

        public override void Enable(string name)
        {
            if (InternalItems.TryGetValue(name, out Detour value))
            {
                Enable(value);
            }
        }

        /// <summary>
        ///     Enables <see langword="this" /> instance.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="item" />
        /// </exception>
        public override void Enable(Detour item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!item.IsEnabled)
            {
                item.Enable(false);
            }
        }

        /// <summary>
        ///     The <see langword="internal" /> items
        /// </summary>
        /// <value>
        ///     The <see langword="internal" /> items.
        /// </value>
        protected override ConcurrentDictionary<string, Detour> InternalItems { get; } =
            new ConcurrentDictionary<string, Detour>();

        /// <summary>
        ///     Gets the process plus.
        /// </summary>
        /// <value>
        ///     The process plus.
        /// </value>
        protected IMemory ProcessPlus { get; }

        static DetourManager() => ProcessorArchitecture =
            IntPtr.Size == 8 ? ProcessorArchitecture.X64 : ProcessorArchitecture.X86;

        private static readonly ProcessorArchitecture ProcessorArchitecture;
    }
}