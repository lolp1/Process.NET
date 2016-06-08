using System;
using System.Collections.Generic;
using System.Linq;
using Process.NET.Extensions;
using Process.NET.Native;
using Process.NET.Native.Types;
using ProcessModule = System.Diagnostics.ProcessModule;
using ProcessThread = System.Diagnostics.ProcessThread;
using SystemProcess = System.Diagnostics.Process;

namespace Process.NET.Utilities
{
    public static class ProcessHelper
    {
        // ReSharper disable once InconsistentNaming
        private const string SE_DEBUG_NAME = "SeDebugPrivilege";

        /// <summary>
        ///     Gets all top-level windows on the screen.
        /// </summary>
        public static IEnumerable<IntPtr> TopLevelWindows => WindowHelper.EnumTopLevelWindows();

        /// <summary>
        ///     Gets all the windows on the screen.
        /// </summary>
        public static IEnumerable<IntPtr> Windows => WindowHelper.EnumAllWindows();

        /// <summary>
        ///     Gets the first process found matching the process name.
        /// </summary>
        /// <param name="processName">Name of the process.</param>
        /// <returns>IntPtr.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IntPtr GetMainWindowHandle(string processName)
        {
            var firstOrDefault = SystemProcess.GetProcessesByName(processName);

            var process = firstOrDefault.FirstOrDefault();

            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            return process.MainWindowHandle;
        }

        /// <summary>
        ///     Returns a new <see cref="System.Diagnostics.Process" /> component, given the identifier of a process.
        /// </summary>
        /// <param name="processId">The system-unique identifier of a process resource.</param>
        /// <returns>
        ///     A <see cref="System.Diagnostics.Process" /> component that is associated with the local process resource
        ///     identified by the processId parameter.
        /// </returns>
        public static SystemProcess FromProcessId(int processId)
        {
            return SystemProcess.GetProcessById(processId);
        }

        /// <summary>
        ///     Creates a collection of new <see cref="Process" /> components and associates them with all the process resources
        ///     that share the specified class name.
        /// </summary>
        /// <param name="className">The class name string.</param>
        /// <returns>
        ///     A collection of type <see cref="Process" /> that represents the process resources running the specified
        ///     application or file.
        /// </returns>
        public static IEnumerable<SystemProcess> FromWindowClassName(string className)
        {
            return Windows.Where(window => WindowHelper.GetClassName(window) == className).Select(FromWindowHandle);
        }

        /// <summary>
        ///     Retrieves a new <see cref="Process" /> component that created the window.
        /// </summary>
        /// <param name="windowHandle">A handle to the window.</param>
        /// <returns>
        ///     A <see cref="Process" />A <see cref="Process" /> component that is associated with the specified window
        ///     handle.
        /// </returns>
        public static SystemProcess FromWindowHandle(IntPtr windowHandle)
        {
            return FromProcessId(WindowHelper.GetWindowProcessId(windowHandle));
        }

        /// <summary>
        ///     Creates a collection of new <see cref="Process" /> components and associates them with all the process resources
        ///     that share the specified window title.
        /// </summary>
        /// <param name="windowTitle">The window title string.</param>
        /// <returns>
        ///     A collection of type <see cref="Process" /> that represents the process resources running the specified
        ///     application or file.
        /// </returns>
        public static IEnumerable<SystemProcess> FromWindowTitle(string windowTitle)
        {
            return Windows.Where(window => WindowHelper.GetWindowText(window) == windowTitle).Select(FromWindowHandle);
        }

        /// <summary>
        ///     Creates a collection of new <see cref="Process" /> components and associates them with all the process resources
        ///     that contain the specified window title.
        /// </summary>
        /// <param name="windowTitle">A part a window title string.</param>
        /// <returns>
        ///     A collection of type <see cref="Process" /> that represents the process resources running the specified
        ///     application or file.
        /// </returns>
        public static IEnumerable<SystemProcess> FromWindowTitleContains(string windowTitle)
        {
            return
                Windows.Where(window => WindowHelper.GetWindowText(window).Contains(windowTitle))
                    .Select(FromWindowHandle);
        }

        public static IEnumerable<SystemProcess> CollectFromInternalName(string name)
        {
            return
                SystemProcess.GetProcesses()
                    .ToArray()
                    .Where(
                        process =>
                            !string.IsNullOrEmpty(process.MainModule.FileVersionInfo.InternalName) &&
                            string.Equals(process.MainModule.FileVersionInfo.InternalName, name,
                                StringComparison.CurrentCultureIgnoreCase));
        }


        public static IEnumerable<SystemProcess> FindProcessesByProductName(string name)
        {
            var processes = new List<SystemProcess>(SystemProcess.GetProcesses());
            return processes.Where(process =>
                !string.IsNullOrEmpty(process.MainModule.FileVersionInfo.ProductName) &&
                string.Equals(process.MainModule.FileVersionInfo.ProductName, name,
                    StringComparison.CurrentCultureIgnoreCase));
        }

        public static SystemProcess FromName(string name)
        {
            var firstOrDefault = SystemProcess.GetProcessesByName(name);

            var process = firstOrDefault.FirstOrDefault();

            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            return process;
        }

        public static SystemProcess GetByInternalName(string name)
        {
            return CollectFromInternalName(name).FirstOrDefault();
        }

        public static SystemProcess GetByProductName(string name)
        {
            using (var firstOrDefault = FindProcessesByProductName(name).FirstOrDefault())
            {
                if (firstOrDefault == null)
                {
                    throw new NullReferenceException($"Process {name} not found.");
                }
                return firstOrDefault;
            }
        }

        public static SystemProcess SelectFromName(string internalName)
        {
            for (;;)
            {
                var processList = CollectFromInternalName(internalName).ToList();
                if (processList.Count == 0)
                    throw new Exception($"No '{internalName}' processes found");

                try
                {
                    if (processList.Count == 1)
                        return processList[0];
                    Console.WriteLine("Select process:");
                    for (var i = 0; i < processList.Count; ++i)
                    {
                        var debugging = false;
                        Kernel32.CheckRemoteDebuggerPresent(processList[i].Handle, ref debugging);

                        Console.WriteLine(
                            $"[{i}] {processList[i].GetVersionInfo()} PID: {processList[i].Id} {(debugging ? "(Already debugging)" : "")}");
                    }

                    Console.WriteLine();
                    Console.Write("> ");
                    var index = Convert.ToInt32(Console.ReadLine());

                    return processList[index];
                }
                catch (Exception ex)
                {
                    if (processList.Count == 1)
                        throw new Exception(ex.Message);
                }
            }
        }

        public static bool SetDebugPrivileges()
        {
            IntPtr hToken;
            LUID luidSeDebugNameValue;
            TOKEN_PRIVILEGES tkpPrivileges;

            if (
                !Advapi32.OpenProcessToken(Kernel32.GetCurrentProcess(),
                    TokenObject.TOKEN_ADJUST_PRIVILEGES | TokenObject.TOKEN_QUERY, out hToken))
                return false;

            if (!Advapi32.LookupPrivilegeValue(null, SE_DEBUG_NAME, out luidSeDebugNameValue))
            {
                Kernel32.CloseHandle(hToken);
                return false;
            }

            tkpPrivileges.PrivilegeCount = 1;
            tkpPrivileges.Luid = luidSeDebugNameValue;
            tkpPrivileges.Attributes = PrivilegeAttributes.SE_PRIVILEGE_ENABLED;

            if (Advapi32.AdjustTokenPrivileges(hToken, false, ref tkpPrivileges, 0, IntPtr.Zero, IntPtr.Zero))
                return Kernel32.CloseHandle(hToken);
            Kernel32.CloseHandle(hToken);
            return false;
        }
    }
}