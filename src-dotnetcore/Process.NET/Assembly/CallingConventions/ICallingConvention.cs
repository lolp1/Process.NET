﻿using System;
using Process.NET.Native.Types;

namespace Process.NET.Assembly.CallingConventions
{
    /// <summary>
    ///     Interface defining a calling convention.
    /// </summary>
    public interface ICallingConvention
    {
        /// <summary>
        ///     The name of the calling convention.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Defines which function performs the clean-up task.
        /// </summary>
        CleanupTypes Cleanup { get; }

        /// <summary>
        ///     Formats the given parameters to call a function.
        /// </summary>
        /// <param name="parameters">An array of parameters.</param>
        /// <returns>The mnemonics to pass the parameters.</returns>
        string FormatParameters(IntPtr[] parameters);

        /// <summary>
        ///     Formats the call of a given function.
        /// </summary>
        /// <param name="function">The function to call.</param>
        /// <returns>The mnemonics to call the function.</returns>
        string FormatCalling(IntPtr function);

        /// <summary>
        ///     Formats the cleaning of a given number of parameters.
        /// </summary>
        /// <param name="nbParameters">The number of parameters to clean.</param>
        /// <returns>The mnemonics to clean a given number of parameters.</returns>
        string FormatCleaning(int nbParameters);
    }
}