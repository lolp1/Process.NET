using System;

namespace Process.NET.Assembly.Assemblers
{
    /// <summary>
    ///     Interface defining an assembler.
    /// </summary>
    public interface IAssembler
    {
        /// <summary>
        ///     Assemble the specified assembly code.
        /// </summary>
        /// <param name="asm">The assembly code.</param>
        /// <returns>An array of bytes containing the assembly code.</returns>
        byte[] Assemble(string asm);

        /// <summary>
        ///     Assemble the specified assembly code at a base address.
        /// </summary>
        /// <param name="asm">The assembly code.</param>
        /// <param name="baseAddress">The address where the code is rebased.</param>
        /// <returns>An array of bytes containing the assembly code.</returns>
        byte[] Assemble(string asm, IntPtr baseAddress);
    }
}