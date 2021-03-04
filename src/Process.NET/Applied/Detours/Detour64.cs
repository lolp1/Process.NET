using System;
using System.Collections.Generic;
using Process.NET.Extensions;
using Process.NET.Memory;

namespace Process.NET.Applied.Detours
{
    /// <summary>
    ///     A manager class to handle function detours, and hooks.
    ///     <remarks>
    ///         All
    ///         credits to the GreyMagic library written by Apoc @ www.ownedcore.com
    ///     </remarks>
    /// </summary>
    public class Detour64 : Detour
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Detour64" /> class.
        /// </summary>
        /// <param name="target">The target delegate.</param>
        /// <param name="hook">The hook delegate.</param>
        /// <param name="identifier"></param>
        /// <param name="memory">The <see cref="MemoryPlus" /> instance.</param>
        /// <param name="ignoreRules"></param>
        public Detour64(Delegate target, Delegate hook, string identifier, IMemory memory, bool ignoreRules = false)
        {
            ProcessMemory = memory;
            Identifier = identifier;
            IgnoreRules = ignoreRules;
            TargetDelegate = target;
            Target = target.ToFunctionPtr();
            HookDelegate = hook;
            HookPointer = hook.ToFunctionPtr();
            Original = new List<byte>();
            Original.AddRange(memory.Read(Target, X64JumpDetourInstructionsBytes.Length));
            var bytes = BitConverter.GetBytes(HookPointer.ToInt64());
            New = new List<byte> {0x50, 0x48, 0xB8};
            New.AddRange(bytes);
            New.AddRange(new byte[] {0x50, 0x48, 0x8B, 0x44, 0x24, 0x8, 0xC2, 0x8, 0x0});
        }

        private byte[] X64JumpDetourInstructionsBytes { get; } =
        {
            0x50,
            0x48,
            0xB8,
            0x90,
            0x90,
            0x90,
            0x90,
            0x90,
            0x90,
            0x90,
            0x90,
            0x50,
            0x48,
            0x8B,
            0x44,
            0x24,
            0x8,
            0xC2,
            0x8,
            0x0
        };
    }
}