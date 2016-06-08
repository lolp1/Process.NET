using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Process.NET.Native.Types;
using Process.NET.Utilities;

namespace Process.NET.Extensions
{
    public static class ProcessExtensions
    {
        public static IList<ProcessThread> GetThreads(this System.Diagnostics.Process process)
        {
            return process.Threads.Cast<ProcessThread>().ToList();
        }

        public static IList<ProcessModule> GetModules(this System.Diagnostics.Process process)
        {
            return process.Modules.Cast<ProcessModule>().ToList();
        }

        public static SafeMemoryHandle Open(this System.Diagnostics.Process process, ProcessAccessFlags processAccessFlags = ProcessAccessFlags.AllAccess)
        {
            return MemoryHelper.OpenProcess(processAccessFlags, process.Id);
        }

        public static string GetVersionInfo(this System.Diagnostics.Process process)
        {
            return
                $"{process.MainModule.FileVersionInfo.FileDescription} {process.MainModule.FileVersionInfo.FileMajorPart}.{process.MainModule.FileVersionInfo.FileMinorPart}.{process.MainModule.FileVersionInfo.FileBuildPart} {process.MainModule.FileVersionInfo.FilePrivatePart}";
        }

    }
}
