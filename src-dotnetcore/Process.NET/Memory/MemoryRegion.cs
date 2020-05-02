using System;
using Process.NET.Native.Types;
using Process.NET.Utilities;

namespace Process.NET.Memory
{
    /// <summary>
    ///     Represents a contiguous block of memory in the remote process.
    /// </summary>
    public class MemoryRegion : MemoryPointer, IEquatable<MemoryRegion>
    {
        public MemoryRegion(IProcess processPlus, IntPtr baseAddress) : base(processPlus, baseAddress)
        {
        }

        /// <summary>
        ///     Contains information about the memory.
        /// </summary>
        public MemoryBasicInformation Information => MemoryHelper.Query(Process.Handle, BaseAddress);

        /// <summary>
        ///     Gets if the <see cref="MemoryRegion" /> is valid.
        /// </summary>
        public override bool IsValid => base.IsValid && Information.State != MemoryStateFlags.Free;

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        public bool Equals(MemoryRegion other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) ||
                   (BaseAddress.Equals(other.BaseAddress) && Process.Equals(other.Process) &&
                    Information.RegionSize.Equals(other.Information.RegionSize));
        }

        /// <summary>
        ///     Changes the protection of the n next bytes in remote process.
        /// </summary>
        /// <param name="protection">The new protection to apply.</param>
        /// <param name="mustBeDisposed">The resource will be automatically disposed when the finalizer collects the object.</param>
        /// <returns>A new instance of the <see cref="MemoryProtection" /> class.</returns>
        public MemoryProtection ChangeProtection(
            MemoryProtectionFlags protection = MemoryProtectionFlags.ExecuteReadWrite, bool mustBeDisposed = true)
        {
            return new MemoryProtection(Process.Handle, BaseAddress, Information.RegionSize, protection, mustBeDisposed);
        }

        /// <summary>
        ///     Determines whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((MemoryRegion) obj);
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        public override int GetHashCode()
        {
            return BaseAddress.GetHashCode() ^ Process.GetHashCode() ^ Information.RegionSize.GetHashCode();
        }

        public static bool operator ==(MemoryRegion left, MemoryRegion right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MemoryRegion left, MemoryRegion right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        ///     Releases the memory used by the region.
        /// </summary>
        public void Release()
        {
            // Release the memory
            MemoryHelper.Free(Process.Handle, BaseAddress);
            // Remove the pointer
            BaseAddress = IntPtr.Zero;
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return
                $"BaseAddress = 0x{BaseAddress.ToInt64():X} Size = 0x{Information.RegionSize:X} Protection = {Information.Protect}";
        }
    }
}