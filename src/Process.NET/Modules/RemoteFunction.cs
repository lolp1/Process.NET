using System;
using System.Runtime.InteropServices;
using Process.NET.Memory;

namespace Process.NET.Modules
{
    /// <summary>
    ///     Class representing a function in the remote process.
    /// </summary>
    public class RemoteFunction : MemoryPointer, IProcessFunction
    {
        public RemoteFunction(IProcess processPlus, IntPtr address, string functionName) : base(processPlus, address)
        {
            // Save the parameter
            Name = functionName;
        }

        /// <summary>
        ///     The name of the function.
        /// </summary>
        public string Name { get; }

        public T GetDelegate<T>()
        {
            return Marshal.GetDelegateForFunctionPointer<T>(BaseAddress);
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return $"BaseAddress = 0x{BaseAddress.ToInt64():X} Name = {Name}";
        }
    }
}