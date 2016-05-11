using System;
using System.Runtime.InteropServices;
using System.Security;
using Process.NET.Native.Types;

namespace Process.NET.Native
{
    public static class Kernel32
    {
        static Kernel32()
        {
            var pKernel32 = GetModuleHandle("kernel32.dll");
            if (pKernel32 == IntPtr.Zero)
                throw new Exception("Failed to get kernel32.dll module handle");

            var procAddress = GetProcAddress(pKernel32, "IsWow64Process");
            Is32BitSystem = procAddress == IntPtr.Zero;
        }

        public static bool Is32BitSystem { get; }

        /// <summary>
        ///     Removes a hook procedure installed in a hook chain by the SetWindowsHookEx function.
        /// </summary>
        /// <param name="hhk">
        ///     A handle to the hook to be removed. This parameter is a hook handle obtained by a previous call to
        ///     SetWindowsHookEx.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get
        ///     extended error information, call GetLastError.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int UnhookWindowsHookEx(IntPtr hhk);

        /// <summary>
        ///     Passes the hook information to the next hook procedure in the current hook chain. A hook procedure can call this
        ///     function either before or after processing the hook information.
        /// </summary>
        /// <param name="hhk">This parameter is ignored.</param>
        /// <param name="nCode">
        ///     The hook code passed to the current hook procedure. The next hook procedure uses this code to
        ///     determine how to process the hook information.
        /// </param>
        /// <param name="wParam">
        ///     The wParam value passed to the current hook procedure. The meaning of this parameter depends on
        ///     the type of hook associated with the current hook chain.
        /// </param>
        /// <param name="lParam">
        ///     The lParam value passed to the current hook procedure. The meaning of this parameter depends on
        ///     the type of hook associated with the current hook chain.
        /// </param>
        /// <returns>
        ///     This value is returned by the next hook procedure in the chain. The current hook procedure must also return
        ///     this value. The meaning of the return value depends on the hook type. For more information, see the descriptions of
        ///     the individual hook procedures.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        public static extern unsafe void MoveMemory(void* dest, void* src, int size);

        [DllImport("kernel32.dll")]
        public static extern void GetSystemInfo(out SystemInfo input);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process([In] IntPtr process, [Out] out bool wow64Process);

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GetProcAddress")]
        public static extern IntPtr GetProcAddressOrdinal(uint hModule, uint procName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr LoadLibraryA(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DebugActiveProcess(int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DebugActiveProcessStop(int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DebugSetProcessKillOnExit(bool KillOnExit);

        [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern unsafe SafeLoadLibrary LoadLibraryExW([In] string lpwLibFileName, [In] void* hFile,
            [In] uint dwFlags);

        public static bool Is32BitProcess(IntPtr hProcess)
        {
            if (Is32BitSystem)
                return true;

            bool isWow64;
            return IsWow64Process(hProcess, out isWow64) && isWow64;
        }

        [DllImport("kernel32.dll")]
        public static extern bool FindClose(IntPtr hFindFile);

        [DllImport("Kernel32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess,
            [MarshalAs(UnmanagedType.Bool)] ref bool isDebuggerPresent);

        /// <summary>
        ///     Suspends the specified thread. A 64-bit application can suspend a WOW64 thread using the Wow64SuspendThread
        ///     function.
        /// </summary>
        /// <param name="hThread">
        ///     A handle to the thread that is to be suspended.
        ///     The handle must have the THREAD_SUSPEND_RESUME access right. For more information, see Thread Security and Access
        ///     Rights.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is the thread's previous suspend count; otherwise, it is (DWORD) -1.
        ///     To get extended error information, use <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint SuspendThread(SafeMemoryHandle hThread);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetCurrentProcess();

        /// <summary>
        ///     Terminates a thread.
        /// </summary>
        /// <param name="hThread">
        ///     A handle to the thread to be terminated.
        ///     The handle must have the <see cref="ThreadAccessFlags.Terminate" /> access right. For more information, see Thread
        ///     Security and Access Rights.
        /// </param>
        /// <param name="dwExitCode">
        ///     The exit code for the thread. Use the GetExitCodeThread function to retrieve a thread's exit
        ///     value.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.
        ///     If the function fails, the return value is zero. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool TerminateThread(SafeMemoryHandle hThread, int dwExitCode);

        /// <summary>
        ///     Reserves or commits a region of memory within the virtual address space of a specified process. The function
        ///     initializes the memory it allocates to zero, unless MEM_RESET is used.
        ///     To specify the NUMA node for the physical memory, see VirtualAllocExNuma.
        /// </summary>
        /// <param name="hProcess">
        ///     The handle to a process. The function allocates memory within the virtual address space of this process.
        ///     The handle must have the PROCESS_VM_OPERATION access right. For more information, see Process Security and Access
        ///     Rights.
        /// </param>
        /// <param name="lpAddress">
        ///     The pointer that specifies a desired starting address for the region of pages that you want to allocate.
        ///     If you are reserving memory, the function rounds this address down to the nearest multiple of the allocation
        ///     granularity.
        ///     If you are committing memory that is already reserved, the function rounds this address down to the nearest page
        ///     boundary.
        ///     To determine the size of a page and the allocation granularity on the host computer, use the GetSystemInfo
        ///     function.
        /// </param>
        /// <param name="dwSize">
        ///     The size of the region of memory to allocate, in bytes.
        ///     If lpAddress is NULL, the function rounds dwSize up to the next page boundary.
        ///     If lpAddress is not NULL, the function allocates all pages that contain one or more bytes in the range from
        ///     lpAddress to lpAddress+dwSize.
        ///     This means, for example, that a 2-byte range that straddles a page boundary causes the function to allocate both
        ///     pages.
        /// </param>
        /// <param name="flAllocationType">[Flags] The type of memory allocation.</param>
        /// <param name="flProtect">[Flags] The memory protection for the region of pages to be allocated.</param>
        /// <returns>
        ///     If the function succeeds, the return value is the base address of the allocated region of pages.
        ///     If the function fails, the return value is NULL. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(SafeMemoryHandle hProcess, IntPtr lpAddress, int dwSize,
            MemoryAllocationFlags flAllocationType, MemoryProtectionFlags flProtect);

        /// <summary>
        ///     Releases, decommits, or releases and decommits a region of memory within the virtual address space of a specified
        ///     process.
        /// </summary>
        /// <param name="hProcess">
        ///     A handle to a process. The function frees memory within the virtual address space of the process.
        ///     The handle must have the PROCESS_VM_OPERATION access right. For more information, see Process Security and Access
        ///     Rights.
        /// </param>
        /// <param name="lpAddress">
        ///     A pointer to the starting address of the region of memory to be freed.
        ///     If the dwFreeType parameter is MEM_RELEASE, lpAddress must be the base address returned by the
        ///     <see cref="VirtualAllocEx" /> function when the region is reserved.
        /// </param>
        /// <param name="dwSize">
        ///     The size of the region of memory to free, in bytes.
        ///     If the dwFreeType parameter is MEM_RELEASE, dwSize must be 0 (zero).
        ///     The function frees the entire region that is reserved in the initial allocation call to
        ///     <see cref="VirtualAllocEx" />.
        ///     If dwFreeType is MEM_DECOMMIT, the function decommits all memory pages that contain one or more bytes in the range
        ///     from the lpAddress parameter to (lpAddress+dwSize).
        ///     This means, for example, that a 2-byte region of memory that straddles a page boundary causes both pages to be
        ///     decommitted.
        ///     If lpAddress is the base address returned by VirtualAllocEx and dwSize is 0 (zero), the function decommits the
        ///     entire region that is allocated by <see cref="VirtualAllocEx" />.
        ///     After that, the entire region is in the reserved state.
        /// </param>
        /// <param name="dwFreeType">[Flags] The type of free operation.</param>
        /// <returns>
        ///     If the function succeeds, the return value is a nonzero value.
        ///     If the function fails, the return value is 0 (zero). To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool VirtualFreeEx(SafeMemoryHandle hProcess, IntPtr lpAddress, int dwSize,
            MemoryReleaseFlags dwFreeType);

        /// <summary>
        ///     Changes the protection on a region of committed pages in the virtual address space of a specified process.
        /// </summary>
        /// <param name="hProcess">
        ///     A handle to the process whose memory protection is to be changed. The handle must have the PROCESS_VM_OPERATION
        ///     access right.
        ///     For more information, see Process Security and Access Rights.
        /// </param>
        /// <param name="lpAddress">
        ///     A pointer to the base address of the region of pages whose access protection attributes are to be changed.
        ///     All pages in the specified region must be within the same reserved region allocated when calling the VirtualAlloc
        ///     or VirtualAllocEx function using MEM_RESERVE.
        ///     The pages cannot span adjacent reserved regions that were allocated by separate calls to VirtualAlloc or
        ///     <see cref="VirtualAllocEx" /> using MEM_RESERVE.
        /// </param>
        /// <param name="dwSize">
        ///     The size of the region whose access protection attributes are changed, in bytes.
        ///     The region of affected pages includes all pages containing one or more bytes in the range from the lpAddress
        ///     parameter to (lpAddress+dwSize).
        ///     This means that a 2-byte range straddling a page boundary causes the protection attributes of both pages to be
        ///     changed.
        /// </param>
        /// <param name="flNewProtect">
        ///     The memory protection option. This parameter can be one of the memory protection constants.
        ///     For mapped views, this value must be compatible with the access protection specified when the view was mapped (see
        ///     MapViewOfFile, MapViewOfFileEx, and MapViewOfFileExNuma).
        /// </param>
        /// <param name="lpflOldProtect">
        ///     A pointer to a variable that receives the previous access protection of the first page in the specified region of
        ///     pages.
        ///     If this parameter is NULL or does not point to a valid variable, the function fails.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.
        ///     If the function fails, the return value is zero. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(SafeMemoryHandle hProcess, IntPtr lpAddress, int dwSize,
            MemoryProtectionFlags flNewProtect, out MemoryProtectionFlags lpflOldProtect);

        /// <summary>
        ///     Retrieves information about a range of pages within the virtual address space of a specified process.
        /// </summary>
        /// <param name="hProcess">
        ///     A handle to the process whose memory information is queried.
        ///     The handle must have been opened with the PROCESS_QUERY_INFORMATION access right, which enables using the handle to
        ///     read information from the process object.
        ///     For more information, see Process Security and Access Rights.
        /// </param>
        /// <param name="lpAddress">
        ///     A pointer to the base address of the region of pages to be queried.
        ///     This value is rounded down to the next page boundary.
        ///     To determine the size of a page on the host computer, use the GetSystemInfo function.
        ///     If lpAddress specifies an address above the highest memory address accessible to the process, the function fails
        ///     with ERROR_INVALID_PARAMETER.
        /// </param>
        /// <param name="lpBuffer">
        ///     [Out] A pointer to a <see cref="MemoryBasicInformation" /> structure in which information about
        ///     the specified page range is returned.
        /// </param>
        /// <param name="dwLength">The size of the buffer pointed to by the lpBuffer parameter, in bytes.</param>
        /// <returns>
        ///     The return value is the actual number of bytes returned in the information buffer.
        ///     If the function fails, the return value is zero. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int VirtualQueryEx(SafeMemoryHandle hProcess, IntPtr lpAddress,
            out MemoryBasicInformation lpBuffer, int dwLength);

        /// <summary>
        ///     Waits until the specified object is in the signaled state or the time-out interval elapses.
        ///     To enter an alertable wait state, use the WaitForSingleObjectEx function. To wait for multiple objects, use the
        ///     WaitForMultipleObjects.
        /// </summary>
        /// <param name="hHandle">
        ///     A handle to the object. For a list of the object types whose handles can be specified, see the following Remarks
        ///     section.
        ///     If this handle is closed while the wait is still pending, the function's behavior is undefined.
        ///     The handle must have the SYNCHRONIZE access right. For more information, see Standard Access Rights.
        /// </param>
        /// <param name="dwMilliseconds">
        ///     The time-out interval, in milliseconds. If a nonzero value is specified, the function waits until the object is
        ///     signaled or the interval elapses.
        ///     If dwMilliseconds is zero, the function does not enter a wait state if the object is not signaled; it always
        ///     returns immediately.
        ///     If dwMilliseconds is INFINITE, the function will return only when the object is signaled.
        /// </param>
        /// <returns>If the function succeeds, the return value indicates the event that caused the function to return.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern WaitValues WaitForSingleObject(SafeMemoryHandle hHandle, uint dwMilliseconds);

        /// <summary>
        ///     Writes data to an area of memory in a specified process. The entire area to be written to must be accessible or the
        ///     operation fails.
        /// </summary>
        /// <param name="hProcess">
        ///     A handle to the process memory to be modified. The handle must have PROCESS_VM_WRITE and
        ///     PROCESS_VM_OPERATION access to the process.
        /// </param>
        /// <param name="lpBaseAddress">
        ///     A pointer to the base address in the specified process to which data is written. Before data transfer occurs, the
        ///     system verifies that
        ///     all data in the base address and memory of the specified size is accessible for write access, and if it is not
        ///     accessible, the function fails.
        /// </param>
        /// <param name="lpBuffer">
        ///     A pointer to the buffer that contains data to be written in the address space of the specified
        ///     process.
        /// </param>
        /// <param name="nSize">The number of bytes to be written to the specified process.</param>
        /// <param name="lpNumberOfBytesWritten">
        ///     A pointer to a variable that receives the number of bytes transferred into the specified process.
        ///     This parameter is optional. If lpNumberOfBytesWritten is NULL, the parameter is ignored.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.
        ///     If the function fails, the return value is zero. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteProcessMemory(SafeMemoryHandle hProcess, IntPtr lpBaseAddress, byte[] lpBuffer,
            int nSize, out int lpNumberOfBytesWritten);

        /// <summary>
        ///     Sets the context for the specified thread. A 64-bit application can set the context of a WOW64 thread using the
        ///     Wow64SetThreadContext function.
        /// </summary>
        /// <param name="hThread">
        ///     A handle to the thread whose context is to be set. The handle must have the
        ///     <see cref="ThreadAccessFlags.SetContext" /> access right to the thread.
        ///     For more information, see Thread Security and Access Rights.
        /// </param>
        /// <param name="lpContext">
        ///     A pointer to a <see cref="ThreadContext" /> structure that contains the context to be set in the specified thread.
        ///     The value of the ContextFlags member of this structure specifies which portions of a thread's context to set.
        ///     Some values in the <see cref="ThreadContext" /> structure that cannot be specified are silently set to the correct
        ///     value.
        ///     This includes bits in the CPU status register that specify the privileged processor mode, global enabling bits in
        ///     the debugging register,
        ///     and other states that must be controlled by the operating system.
        /// </param>
        /// <returns>
        ///     If the context was set, the return value is nonzero.
        ///     If the function fails, the return value is zero. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetThreadContext(SafeMemoryHandle hThread,
            [MarshalAs(UnmanagedType.Struct)] ref ThreadContext lpContext);

        /// <summary>
        ///     Reads data from an area of memory in a specified process. The entire area to be read must be accessible or the
        ///     operation fails.
        /// </summary>
        /// <param name="hProcess">
        ///     A handle to the process with memory that is being read. The handle must have PROCESS_VM_READ
        ///     access to the process.
        /// </param>
        /// <param name="lpBaseAddress">
        ///     A pointer to the base address in the specified process from which to read. Before any data transfer occurs,
        ///     the system verifies that all data in the base address and memory of the specified size is accessible for read
        ///     access,
        ///     and if it is not accessible the function fails.
        /// </param>
        /// <param name="lpBuffer">
        ///     [Out] A pointer to a buffer that receives the contents from the address space of the specified
        ///     process.
        /// </param>
        /// <param name="dwSize">The number of bytes to be read from the specified process.</param>
        /// <param name="lpNumberOfBytesRead">
        ///     [Out] A pointer to a variable that receives the number of bytes transferred into the specified buffer. If
        ///     lpNumberOfBytesRead is NULL, the parameter is ignored.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.
        ///     If the function fails, the return value is zero. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadProcessMemory(SafeMemoryHandle hProcess, IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        /// <summary>
        ///     Decrements a thread's suspend count. When the suspend count is decremented to zero, the execution of the thread is
        ///     resumed.
        /// </summary>
        /// <param name="hThread">
        ///     A handle to the thread to be restarted.
        ///     This handle must have the <see cref="ThreadAccessFlags.SuspendResume" /> access right. For more information, see
        ///     Thread Security and Access Rights.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is the thread's previous suspend count.
        ///     If the function fails, the return value is (DWORD) -1. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint ResumeThread(SafeMemoryHandle hThread);

        /// <summary>
        ///     Opens an existing local process object.
        /// </summary>
        /// <param name="dwDesiredAccess">
        ///     [Flags] he access to the process object. This access right is checked against the security descriptor for the
        ///     process. This parameter can be one or more of the process access rights.
        ///     If the caller has enabled the SeDebugPrivilege privilege, the requested access is granted regardless of the
        ///     contents of the security descriptor.
        /// </param>
        /// <param name="bInheritHandle">
        ///     If this value is TRUE, processes created by this process will inherit the handle.
        ///     Otherwise, the processes do not inherit this handle.
        /// </param>
        /// <param name="dwProcessId">The identifier of the local process to be opened.</param>
        /// <returns>
        ///     If the function succeeds, the return value is an open handle to the specified process.
        ///     If the function fails, the return value is NULL. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeMemoryHandle OpenProcess(ProcessAccessFlags dwDesiredAccess,
            [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        /// <summary>
        ///     Opens an existing thread object.
        /// </summary>
        /// <param name="dwDesiredAccess">
        ///     The access to the thread object. This access right is checked against the security descriptor for the thread. This
        ///     parameter can be one or more of the thread access rights.
        ///     If the caller has enabled the SeDebugPrivilege privilege, the requested access is granted regardless of the
        ///     contents of the security descriptor.
        /// </param>
        /// <param name="bInheritHandle">
        ///     If this value is TRUE, processes created by this process will inherit the handle.
        ///     Otherwise, the processes do not inherit this handle.
        /// </param>
        /// <param name="dwThreadId">The identifier of the thread to be opened.</param>
        /// <returns>
        ///     If the function succeeds, the return value is an open handle to the specified thread.
        ///     If the function fails, the return value is NULL. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeMemoryHandle OpenThread(ThreadAccessFlags dwDesiredAccess,
            [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwThreadId);

        /// <summary>
        ///     Loads the specified module into the address space of the calling process. The specified module may cause other
        ///     modules to be loaded.
        /// </summary>
        /// <param name="lpFileName">
        ///     The name of the module. This can be either a library module (a .dll file) or an executable module (an .exe file).
        ///     The name specified is the file name of the module and is not related to the name stored in the library module
        ///     itself,
        ///     as specified by the LIBRARY keyword in the module-definition (.def) file.
        ///     If the string specifies a full path, the function searches only that path for the module.
        ///     If the string specifies a relative path or a module name without a path, the function uses a standard search
        ///     strategy to find the module; for more information, see the Remarks.
        ///     If the function cannot find the module, the function fails. When specifying a path, be sure to use backslashes (\),
        ///     not forward slashes (/).
        ///     For more information about paths, see Naming a File or Directory.
        ///     If the string specifies a module name without a path and the file name extension is omitted, the function appends
        ///     the default library extension .dll to the module name.
        ///     To prevent the function from appending .dll to the module name, include a trailing point character (.) in the
        ///     module name string.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is a handle to the module.
        ///     If the function fails, the return value is NULL. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        /// <summary>
        ///     Retrieves the termination status of the specified thread.
        /// </summary>
        /// <param name="hThread">
        ///     A handle to the thread.
        ///     The handle must have the <see cref="ThreadAccessFlags.QueryInformation" /> or
        ///     <see cref="ThreadAccessFlags.QueryLimitedInformation" /> access right.
        ///     For more information, see Thread Security and Access Rights.
        /// </param>
        /// <param name="lpExitCode">
        ///     A pointer to a variable to receive the thread termination status. For more information, see
        ///     Remarks.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.
        ///     If the function fails, the return value is zero. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeThread(SafeMemoryHandle hThread, out IntPtr lpExitCode);

        /// <summary>
        ///     Retrieves the address of an exported function or variable from the specified dynamic-link library (DLL).
        /// </summary>
        /// <param name="hModule">
        ///     A handle to the DLL module that contains the function or variable. The LoadLibrary, LoadLibraryEx,
        ///     LoadPackagedLibrary, or GetModuleHandle function returns this handle.
        ///     The GetProcAddress function does not retrieve addresses from modules that were loaded using the
        ///     LOAD_LIBRARY_AS_DATAFILE flag. For more information, see LoadLibraryEx.
        /// </param>
        /// <param name="procName">
        ///     The function or variable name, or the function's ordinal value.
        ///     If this parameter is an ordinal value, it must be in the low-order word; the high-order word must be zero.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is the address of the exported function or variable.
        ///     If the function fails, the return value is NULL. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        /// <summary>
        ///     Retrieves the process identifier of the specified process.
        /// </summary>
        /// <param name="hProcess">
        ///     A handle to the process. The handle must have the PROCESS_QUERY_INFORMATION or PROCESS_QUERY_LIMITED_INFORMATION
        ///     access right.
        ///     For more information, see Process Security and Access Rights.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is a process id to the specified handle.
        ///     If the function fails, the return value is NULL. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessId(SafeMemoryHandle hProcess);

        /// <summary>
        ///     Retrieves the specified system metric or system configuration setting.
        ///     Note that all dimensions retrieved by <see cref="GetSystemMetrics" /> are in pixels.
        /// </summary>
        /// <param name="metric">The system metric or configuration setting to be retrieved.</param>
        /// <returns>
        ///     If the function succeeds, the return value is the requested system metric or configuration setting.
        ///     If the function fails, the return value is 0. <see cref="Marshal.GetLastWin32Error" /> does not provide extended
        ///     error information.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetSystemMetrics(SystemMetrics metric);

        /// <summary>
        ///     Retrieves the context of the specified thread. A 64-bit application can retrieve the context of a WOW64 thread
        ///     using the Wow64GetThreadContext function.
        /// </summary>
        /// <param name="hThread">
        ///     A handle to the thread whose context is to be retrieved. The handle must have
        ///     <see cref="ThreadAccessFlags.GetContext" /> access to the thread.
        ///     For more information, see Thread Security and Access Rights.
        ///     WOW64: The handle must also have <see cref="ThreadAccessFlags.QueryInformation" /> access.
        /// </param>
        /// <param name="lpContext">
        ///     [Ref] A pointer to a <see cref="ThreadContext" /> structure that receives the appropriate context of the specified
        ///     thread.
        ///     The value of the ContextFlags member of this structure specifies which portions of a thread's context are
        ///     retrieved.
        ///     The <see cref="ThreadContext" /> structure is highly processor specific.
        ///     Refer to the WinNT.h header file for processor-specific definitions of this structures and any alignment
        ///     requirements.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.
        ///     If the function fails, the return value is zero. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetThreadContext(SafeMemoryHandle hThread, ref ThreadContext lpContext);

        /// <summary>
        ///     Retrieves a descriptor table entry for the specified selector and thread.
        /// </summary>
        /// <param name="hThread">
        ///     A handle to the thread containing the specified selector.
        ///     The handle must have <see cref="ThreadAccessFlags.QueryInformation" /> access.
        /// </param>
        /// <param name="dwSelector">The global or local selector value to look up in the thread's descriptor tables.</param>
        /// <param name="lpSelectorEntry">
        ///     A pointer to an <see cref="LdtEntry" /> structure that receives a copy of the descriptor table entry
        ///     if the specified selector has an entry in the specified thread's descriptor table.
        ///     This information can be used to convert a segment-relative address to a linear virtual address.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.
        ///     In that case, the structure pointed to by the lpSelectorEntry parameter receives a copy of the specified descriptor
        ///     table entry.
        ///     If the function fails, the return value is zero. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetThreadSelectorEntry(SafeMemoryHandle hThread, uint dwSelector,
            out LdtEntry lpSelectorEntry);

        /// <summary>
        ///     Retrieves the thread identifier of the specified thread.
        /// </summary>
        /// <param name="hThread">
        ///     A handle to the thread. The handle must have the THREAD_QUERY_INFORMATION or THREAD_QUERY_LIMITED_INFORMATION
        ///     access right.
        ///     For more information about access rights, see Thread Security and Access Rights.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is a thread id to the specified handle.
        ///     If the function fails, the return value is zero. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetThreadId(SafeMemoryHandle hThread);

        /// <summary>
        ///     Closes an open object handle.
        /// </summary>
        /// <param name="hObject">A valid handle to an open object.</param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.
        ///     If the function fails, the return value is zero. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        ///     Creates a thread that runs in the virtual address space of another process.
        ///     Use the CreateRemoteThreadEx function to create a thread that runs in the virtual address space of another
        ///     processor and optionally specify extended attributes.
        /// </summary>
        /// <param name="hProcess">
        ///     A handle to the process in which the thread is to be created. The handle must have the PROCESS_CREATE_THREAD,
        ///     PROCESS_QUERY_INFORMATION,
        ///     PROCESS_VM_OPERATION, PROCESS_VM_WRITE, and PROCESS_VM_READ access rights, and may fail without these rights on
        ///     certain platforms.
        ///     For more information, see Process Security and Access Rights.
        /// </param>
        /// <param name="lpThreadAttributes">
        ///     A pointer to a SECURITY_ATTRIBUTES structure that specifies a security descriptor for the new thread and determines
        ///     whether child processes can inherit the returned handle.
        ///     If lpThreadAttributes is NULL, the thread gets a default security descriptor and the handle cannot be inherited.
        ///     The access control lists (ACL) in the default security descriptor for a thread come from the primary token of the
        ///     creator.
        /// </param>
        /// <param name="dwStackSize">
        ///     The initial size of the stack, in bytes. The system rounds this value to the nearest page. If this parameter is 0
        ///     (zero), the new thread uses the default size for the executable.
        ///     For more information, see Thread Stack Size.
        /// </param>
        /// <param name="lpStartAddress">
        ///     A pointer to the application-defined function of type LPTHREAD_START_ROUTINE to be executed by the thread and
        ///     represents the starting address of the thread in the remote process.
        ///     The function must exist in the remote process. For more information, see ThreadProc.
        /// </param>
        /// <param name="lpParameter">A pointer to a variable to be passed to the thread function.</param>
        /// <param name="dwCreationFlags">The flags that control the creation of the thread.</param>
        /// <param name="lpThreadId">
        ///     A pointer to a variable that receives the thread identifier. If this parameter is NULL, the
        ///     thread identifier is not returned.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is a handle to the new thread.
        ///     If the function fails, the return value is NULL. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeMemoryHandle CreateRemoteThread(SafeMemoryHandle hProcess, IntPtr lpThreadAttributes,
            uint dwStackSize, IntPtr lpStartAddress,
            IntPtr lpParameter, ThreadCreationFlags dwCreationFlags, out int lpThreadId);

        /// <summary>
        ///     Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count.
        ///     When the reference count reaches zero, the module is unloaded from the address space of the calling process and the
        ///     handle is no longer valid.
        /// </summary>
        /// <param name="hModule">
        ///     A handle to the loaded library module. The <see cref="LoadLibrary" /> , LoadLibraryEx,
        ///     GetModuleHandle, or GetModuleHandleEx function returns this handle.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.
        ///     If the function fails, the return value is zero. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [SecurityCritical, SecuritySafeCritical]
        internal static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
            return IntPtr.Size == 4 ? GetWindowLong32(hWnd, nIndex) : GetWindowLongPtr64(hWnd, nIndex);
        }

        [SecurityCritical, SecuritySafeCritical]
        public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr newValue)
        {
            return IntPtr.Size == 4
                ? SetWindowLong32(hWnd, nIndex, newValue)
                : SetWindowLongPtr64(hWnd, nIndex, newValue);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        internal static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(HookType code,
            HookProc func,
            IntPtr hInstance,
            int threadId);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        internal static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        internal static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr newValue);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        internal static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr newValue);

        [DllImport("user32.dll")]
        internal static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, int msg, IntPtr wParam,
            IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern int RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }
}