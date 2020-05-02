﻿using System;
using System.Collections.Generic;
using System.Text;
using Process.NET.Native.Types;

namespace Process.NET.Assembly.CallingConventions
{
    /// <summary>
    ///     Define the Fast Calling Convention (aka __msfastcall).
    /// </summary>
    public class FastcallCallingConvention : ICallingConvention
    {
        /// <summary>
        ///     The name of the calling convention.
        /// </summary>
        public string Name => "Fastcall";

        /// <summary>
        ///     Defines which function performs the clean-up task.
        /// </summary>
        public CleanupTypes Cleanup => CleanupTypes.Callee;

        /// <summary>
        ///     Formats the given parameters to call a function.
        /// </summary>
        /// <param name="parameters">An array of parameters.</param>
        /// <returns>The mnemonics to pass the parameters.</returns>
        public string FormatParameters(IntPtr[] parameters)
        {
            // Declare a var to store the mnemonics
            var ret = new StringBuilder();
            var paramList = new List<IntPtr>(parameters);
            // Store the first parameter in the ECX register
            if (paramList.Count > 0)
            {
                ret.AppendLine("mov ecx, " + paramList[0]);
                paramList.RemoveAt(0);
            }
            // Store the second parameter in the EDX register
            if (paramList.Count > 0)
            {
                ret.AppendLine("mov edx, " + paramList[0]);
                paramList.RemoveAt(0);
            }
            // For each parameters (in reverse order)
            paramList.Reverse();
            foreach (var parameter in paramList)
                ret.AppendLine("push " + parameter);
            // Return the mnemonics
            return ret.ToString();
        }

        /// <summary>
        ///     Formats the call of a given function.
        /// </summary>
        /// <param name="function">The function to call.</param>
        /// <returns>The mnemonics to call the function.</returns>
        public string FormatCalling(IntPtr function)
        {
            return "call " + function;
        }

        /// <summary>
        ///     Formats the cleaning of a given number of parameters.
        /// </summary>
        /// <param name="nbParameters">The number of parameters to clean.</param>
        /// <returns>The mnemonics to clean a given number of parameters.</returns>
        public string FormatCleaning(int nbParameters)
        {
            return string.Empty;
        }
    }
}