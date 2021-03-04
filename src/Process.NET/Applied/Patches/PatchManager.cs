using System;
using System.Collections.Concurrent;
using Process.NET.Memory;

namespace Process.NET.Applied.Patches
{
    /// <summary>
    ///     A manager class to handle memory patches.
    /// </summary>
    public class PatchManager : ComplexAppliedManager<Patch>, IDisposable
    {
        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PatchManager" /> class.
        /// </summary>
        /// <param name="processMemory">The process memory.</param>
        public PatchManager(IMemory processMemory) => MemoryBase = processMemory;

        /// <summary>
        ///     Creates a new <see cref="Patch" /> at the specified address.
        /// </summary>
        /// <param name="address">The address to begin the patch.</param>
        /// <param name="patchWith">
        ///     The bytes to be written as the patch.
        /// </param>
        /// <param name="name">The name of the patch.</param>
        /// <returns>
        ///     A patch object that exposes the required methods to apply and remove
        ///     the patch.
        /// </returns>
        public Patch Create(IntPtr address, byte[] patchWith, string name)
        {
            if (InternalItems.ContainsKey(name))
            {
                Remove(name);
            }

            return InternalItems.GetOrAdd(name, new Patch(address, patchWith, name, MemoryBase));
        }

        /// <summary>
        ///     Creates a new <see cref="Patch" /> at the specified address, and
        ///     applies it.
        /// </summary>
        /// <param name="address">The address to begin the patch.</param>
        /// <param name="patchWith">
        ///     The bytes to be written as the patch.
        /// </param>
        /// <param name="name">The name of the patch.</param>
        /// <returns>
        ///     A patch object that exposes the required methods to apply and remove
        ///     the patch.
        /// </returns>
        public Patch CreateAndApply(IntPtr address, byte[] patchWith, string name)
        {
            if (InternalItems.ContainsKey(name))
            {
                Remove(name);
            }

            return Create(address, patchWith, name);
        }

        /// <summary>
        ///     <para>
        ///         Disables <see cref="Process.NET.Applied.AppliedManager`1" />
        ///     </para>
        ///     <para>instance.</para>
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="item" />
        /// </exception>
        public override void Disable(Patch item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.IsEnabled)
            {
                item.Disable(false);
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

            if (InternalItems.TryGetValue(name, out Patch patch))
            {
                Disable(patch);
            }
        }

        public override void Disable(Patch item, bool dueToRules)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.IsEnabled)
            {
                item.Disable(dueToRules);
            }
        }

        public override void Disable(string name, bool dueToRules)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            }

            if (InternalItems.TryGetValue(name, out Patch patch))
            {
                Disable(patch, dueToRules);
            }
        }

        public override void Enable(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            }

            if (InternalItems.TryGetValue(name, out Patch patch))
            {
                Enable(patch);
            }
        }

        /// <summary>
        ///     Enables this instance.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="item" />
        /// </exception>
        public override void Enable(Patch item)
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
        protected override ConcurrentDictionary<string, Patch> InternalItems { get; } =
            new ConcurrentDictionary<string, Patch>();

        /// <summary>
        ///     <para>
        ///         The reference of the
        ///         <see cref="Process.NET.Applied.Patches.PatchManager.MemoryBase" />
        ///     </para>
        ///     <para>
        ///         object.
        ///         <remarks>
        ///             This value is invalid if the manager was created for the
        ///             <see cref="MemorySharp" /> class.
        ///         </remarks>
        ///     </para>
        /// </summary>
        protected IMemory MemoryBase { get; }
    }
}