using System;
using System.Linq;
using System.Text;
using Process.NET.Native.Types;

namespace Process.NET.Assembly.CallingConventions
{
    /// <summary>
    ///     Define the C Declaration Calling Convention.
    /// </summary>
    public class CdeclCallingConvention : ICallingConvention
    {
        /// <summary>
        ///     The name of the calling convention.
        /// </summary>
        public string Name => "Cdecl";

        /// <summary>
        ///     Defines which function performs the clean-up task.
        /// </summary>
        public CleanupTypes Cleanup => CleanupTypes.Caller;

        /// <summary>
        ///     Formats the given parameters to call a function.
        /// </summary>
        /// <param name="parameters">An array of parameters.</param>
        /// <returns>The mnemonics to pass the parameters.</returns>
        public string FormatParameters(IntPtr[] parameters)
        {
            // Declare a var to store the mnemonics
            var ret = new StringBuilder();
            // For each parameters (in reverse order)
            foreach (var parameter in parameters.Reverse())
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
            return "add esp, " + nbParameters*4;
        }
    }
}