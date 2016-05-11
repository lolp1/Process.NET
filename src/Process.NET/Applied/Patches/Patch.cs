using System;
using System.Linq;
using Process.NET.Memory;

namespace Process.NET.Applied.Patches
{
    /// <summary>
    ///     Class representing a Memory Patch object.
    /// </summary>
    public class Patch : IComplexApplied
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Patch" /> class.
        /// </summary>
        /// <param name="address">The address where the patch is located in Memory.</param>
        /// <param name="patchWith">The bytes to be written.</param>
        /// <param name="identifier"></param>
        /// <param name="processPlus"></param>
        /// <param name="ignoreRules"></param>
        public Patch(IntPtr address, byte[] patchWith, string identifier, IMemory processPlus,
            bool ignoreRules = false)
        {
            Identifier = identifier;
            ProcessPlus = processPlus;
            Address = address;
            PatchBytes = patchWith;
            OriginalBytes = processPlus.Read(address, patchWith.Length);
            IgnoreRules = ignoreRules;
        }

        private IMemory ProcessPlus { get; }

        /// <summary>
        ///     Gets the address where the patch is written.
        /// </summary>
        /// <value>The address where the patch is written.</value>
        public IntPtr Address { get; }

        /// <summary>
        ///     Gets the original bytes [bytes before the patch was applied].
        /// </summary>
        /// <value>The original bytes [before the patch was applied].</value>
        public byte[] OriginalBytes { get; }

        /// <summary>
        ///     Gets the bytes after the patch has been applied.
        /// </summary>
        /// <value>The Patched Bytes after the patch has been applied.</value>
        public byte[] PatchBytes { get; }

        /// <summary>
        ///     The name of the memorySharp patch.
        /// </summary>
        /// <value>The name.</value>
        public string Identifier { get; }

        /// <summary>
        ///     Gets a value indicating whether this instance is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled { get; set; }

        /// <summary>
        ///     Get a value indicating if the patch has been disabled due to a running AntiCheat scan
        /// </summary>
        public bool DisabledDueToRules { get; set; }

        /// <summary>
        ///     Geta s value indicating if the patch should never be disabled by the AntiCheat scan logic
        /// </summary>
        public bool IgnoreRules { get; }

        /// <summary>
        ///     Enables this instance.
        /// </summary>
        public void Enable()
        {
            Enable(false);
        }

        /// <summary>
        ///     Disables this instance.
        /// </summary>
        public void Disable()
        {
            Disable(false);
        }

        /// <summary>
        ///     Disables the memorySharp patch.
        /// </summary>
        public void Disable(bool disableDueToRules)
        {
            if (IgnoreRules && disableDueToRules)
                return;

            DisabledDueToRules = disableDueToRules;

            ProcessPlus.Write(Address, OriginalBytes);
            IsEnabled = false;
        }

        /// <summary>
        ///     Enables the memorySharp patch.
        /// </summary>
        public void Enable(bool disableDueToRules)
        {
            if (disableDueToRules && DisabledDueToRules)
            {
                DisabledDueToRules = false;
                ProcessPlus.Write(Address, PatchBytes);
                IsEnabled = true;
            }

            else
            {
                if (DisabledDueToRules)
                    return;

                ProcessPlus.Write(Address, PatchBytes);
                IsEnabled = true;
            }
        }

        public bool MustBeDisposed { get; set; }

        /// <summary>
        ///     Disposes this instance.
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

        ~Patch()
        {
            if (MustBeDisposed)
                Dispose();
        }

        /// <summary>
        ///     States if the Patch is enabled.
        /// </summary>
        /// <returns>
        ///     <c>true</c>
        ///     if this instance is enabled; otherwise, <c>false</c>
        ///     .
        /// </returns>
        public bool CheckIfEnabled()
        {
            return ProcessPlus.Read(Address, PatchBytes.Length).SequenceEqual(PatchBytes);
        }
    }
}