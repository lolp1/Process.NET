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
    public class Detour32 : Detour
    {
        /// <summary>
        ///     This var is not used within the detour itself. It is only here to
        ///     keep a reference, to avoid the <see cref="GC" /> from collecting the
        ///     <see langword="delegate" /> instance!
        /// </summary>
        private readonly Delegate _hookDelegate;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Detour" /> class.
        /// </summary>
        /// <param name="target">The target delegate.</param>
        /// <param name="hook">The hook delegate.</param>
        /// <param name="identifier"></param>
        /// <param name="memory">The <see cref="MemoryPlus" /> instance.</param>
        /// <param name="ignoreRules"></param>
        public Detour32(Delegate target, Delegate hook, string identifier, IMemory memory, bool ignoreRules = false)
        {
            ProcessMemory = memory;
            Identifier = identifier;
            IgnoreRules = ignoreRules;
            TargetDelegate = target;
            Target = target.ToFunctionPtr();
            _hookDelegate = hook;
            HookPointer = hook.ToFunctionPtr();
            Original = new List<byte>();
            Original.AddRange(memory.Read(Target, 6));
            New = new List<byte> {104};
            New.AddRange(BitConverter.GetBytes(HookPointer.ToInt32()));
            New.Add(195);
        }
    }
}