using System;
using System.Runtime.InteropServices;

// ReSharper disable All

namespace Process.NET.Memory
{
    public enum MemoryProtectionType
    {
        Local,
        External
    }

    public class MemoryProtectionOperation : IDisposable
    {
        private readonly IntPtr _hProcess;
        private readonly int _oldProtect;
        private readonly int _size;
        private readonly MemoryProtectionType _type;

        public readonly IntPtr Address;

        public MemoryProtectionOperation(IntPtr hProcess, IntPtr address, int size, int flNewProtect)
        {
            _hProcess = hProcess;
            Address = address;
            _size = size;
            _type = MemoryProtectionType.External;

            VirtualProtectEx(hProcess, Address, size, flNewProtect, out _oldProtect);
        }

        public MemoryProtectionOperation(IntPtr address, int size, int flNewProtect)
        {
            Address = address;
            _size = size;
            _type = MemoryProtectionType.Local;

            VirtualProtect(Address, size, flNewProtect, out _oldProtect);
        }

        public void Dispose()
        {
            int trash;
            if (_type == MemoryProtectionType.Local)
                VirtualProtectEx(_hProcess, Address, _size, _oldProtect, out trash);
            else
                VirtualProtect(Address, _size, _oldProtect, out trash);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualProtect(IntPtr lpAddress, int dwSize, int flNewProtect,
            out int lpflOldProtect);

        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, int flNewProtect,
            out int lpflOldProtect);
    }
}