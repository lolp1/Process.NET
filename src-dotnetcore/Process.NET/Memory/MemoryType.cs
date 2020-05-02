using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Process.NET.Memory
{
    /// <summary>
    /// Defines types of memory manipulations. 
    /// </summary>
    public enum MemoryType
    {
        /// <summary>
        /// The memory is within the local process. Often, this is called "injected" or "Internal".
        /// </summary>
        Local,

        /// <summary>
        /// The memory is not within the local process. Often this is called "remote" or "external".
        /// </summary>
        Remote
    }
}
