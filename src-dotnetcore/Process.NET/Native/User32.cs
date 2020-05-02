using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Process.NET.Native.Types;

namespace Process.NET.Native
{
    public static class User32
    {
        /// <summary>
        ///     Retrieves the name of the class to which the specified window belongs.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="lpClassName">The class name string.</param>
        /// <param name="nMaxCount">
        ///     The length of the lpClassName buffer, in characters.
        ///     The buffer must be large enough to include the terminating null character; otherwise, the class name string is
        ///     truncated to nMaxCount-1 characters.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is the number of characters copied to the buffer, not including the
        ///     terminating null character.
        ///     If the function fails, the return value is zero. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern short GetKeyState(ModiferVirtualKeyStates nVirtKey);

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
        ///     Retrieves the show state and the restored, minimized, and maximized positions of the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="lpwndpl">
        ///     A pointer to the <see cref="WindowPlacement" /> structure that receives the show state and position information.
        ///     Before calling <see cref="GetWindowPlacement" />, set the <see cref="WindowPlacement.Length" /> member.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.
        ///     If the function fails, the return value is zero. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, out WindowPlacement lpwndpl);

        /// <summary>
        ///     Copies the text of the specified window's title bar (if it has one) into a buffer. If the specified window is a
        ///     control, the text of the control is copied.
        /// </summary>
        /// <param name="hWnd">A handle to the window or control containing the text.</param>
        /// <param name="lpString">
        ///     The buffer that will receive the text. If the string is as long or longer than the buffer, the
        ///     string is truncated and terminated with a null character.
        /// </param>
        /// <param name="nMaxCount">
        ///     The maximum number of characters to copy to the buffer, including the null character. If the
        ///     text exceeds this limit, it is truncated.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is the length, in characters, of the copied string, not including the
        ///     terminating null character.
        ///     If the window has no title bar or text, if the title bar is empty, or if the window or control handle is invalid,
        ///     the return value is zero.
        ///     To get extended error information, call <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        /// <summary>
        ///     Retrieves the length, in characters, of the specified window's title bar text (if the window has a title bar).
        ///     If the specified window is a control, the function retrieves the length of the text within the control.
        /// </summary>
        /// <param name="hWnd">A handle to the window or control.</param>
        /// <returns>
        ///     If the function succeeds, the return value is the length, in characters, of the text.
        ///     Under certain conditions, this value may actually be greater than the length of the text.
        ///     If the window has no text, the return value is zero. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        /// <summary>
        ///     Retrieves the identifier of the thread that created the specified window and, optionally, the identifier of the
        ///     process that created the window.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="lpdwProcessId">
        ///     [Out] A pointer to a variable that receives the process identifier.
        ///     If this parameter is not <c>NULL</c>, <see cref="GetWindowThreadProcessId" /> copies the identifier of the process
        ///     to the variable; otherwise, it does not.
        /// </param>
        /// <returns>The return value is the identifier of the thread that created the window.</returns>
        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        /// <summary>
        ///     Enumerates the child windows that belong to the specified parent window by passing the handle to each child window,
        ///     in turn, to an application-defined callback function.
        ///     EnumChildWindows continues until the last child window is enumerated or the callback function returns <c>False</c>.
        /// </summary>
        /// <param name="hwndParent">
        ///     A handle to the parent window whose child windows are to be enumerated.
        ///     If this parameter is <see cref="IntPtr.Zero" />, this function is equivalent to EnumWindows.
        /// </param>
        /// <param name="lpEnumFunc">
        ///     A pointer to an application-defined callback function.
        ///     For more information, see <see cref="EnumWindowsProc" />.
        /// </param>
        /// <param name="lParam">An application-defined value to be passed to the callback function.</param>
        /// <returns>The return value is not used.</returns>
        /// <remarks>
        ///     If a child window has created child windows of its own, <see cref="EnumChildWindows" /> enumerates those
        ///     windows as well.
        /// </remarks>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        /// <summary>
        ///     Flashes the specified window one time. It does not change the active state of the window.
        ///     To flash the window a specified number of times, use the <see cref="FlashWindowEx" /> function.
        /// </summary>
        /// <param name="hwnd">A handle to the window to be flashed. The window can be either open or minimized.</param>
        /// <param name="bInvert">
        ///     If this parameter is <c>True</c>, the window is flashed from one state to the other.
        ///     If it is <c>False</c>, the window is returned to its original state (either active or inactive).
        ///     When an application is minimized and this parameter is <c>True</c>, the taskbar window button flashes
        ///     active/inactive.
        ///     If it is <c>False</c>, the taskbar window button flashes inactive, meaning that it does not change colors.
        ///     It flashes, as if it were being redrawn, but it does not provide the visual invert clue to the user.
        /// </param>
        /// <returns>
        ///     The return value specifies the window's state before the call to the <see cref="FlashWindow" /> function.
        ///     If the window caption was drawn as active before the call, the return value is nonzero. Otherwise, the return value
        ///     is zero.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool FlashWindow(IntPtr hwnd, bool bInvert);

        /// <summary>
        ///     Flashes the specified window. It does not change the active state of the window.
        /// </summary>
        /// <param name="pwfi">A pointer to a <see cref="FlashInfo" /> structure.</param>
        /// <returns>
        ///     The return value specifies the window's state before the call to the <see cref="FlashWindowEx" /> function.
        ///     If the window caption was drawn as active before the call, the return value is nonzero. Otherwise, the return value
        ///     is zero.
        /// </returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FlashWindowEx(ref FlashInfo pwfi);

        /// <summary>
        ///     Translates (maps) a virtual-key code into a scan code or character value, or translates a scan code into a
        ///     virtual-key code.
        ///     To specify a handle to the keyboard layout to use for translating the specified code, use the MapVirtualKeyEx
        ///     function.
        /// </summary>
        /// <param name="key">
        ///     The virtual key code or scan code for a key. How this value is interpreted depends on the value of the uMapType
        ///     parameter.
        /// </param>
        /// <param name="translation">
        ///     The translation to be performed. The value of this parameter depends on the value of the uCode parameter.
        /// </param>
        /// <returns>
        ///     The return value is either a scan code, a virtual-key code, or a character value, depending on the value of uCode
        ///     and uMapType.
        ///     If there is no translation, the return value is zero.
        /// </returns>
        [DllImport("user32")]
        public static extern int MapVirtualKey(int key, TranslationTypes translation);

        /// <summary>
        ///     Synthesizes keystrokes, mouse motions, and button clicks.
        /// </summary>
        /// <param name="nInputs">The number of structures in the pInputs array.</param>
        /// <param name="pInputs">
        ///     An array of <see cref="Input" /> structures. Each structure represents an event to be inserted into the keyboard or
        ///     mouse input stream.
        /// </param>
        /// <param name="cbSize">
        ///     The size, in bytes, of an <see cref="Input" /> structure. If <see cref="cbSize" /> is not the size of an
        ///     <see cref="Input" /> structure, the function fails.
        /// </param>
        /// <returns>
        ///     The function returns the number of events that it successfully inserted into the keyboard or mouse input stream.
        ///     If the function returns zero, the input was already blocked by another thread. To get extended error information,
        ///     call <see cref="Marshal.GetLastWin32Error" />.
        ///     This function fails when it is blocked by UIPI.
        ///     Note that neither <see cref="Marshal.GetLastWin32Error" /> nor the return value will indicate the failure was
        ///     caused by UIPI blocking.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SendInput(int nInputs, Input[] pInputs, int cbSize);

        /// <summary>
        ///     Sends the specified message to a window or windows.
        ///     The SendMessage function calls the window procedure for the specified window and does not return until the window
        ///     procedure has processed the message.
        /// </summary>
        /// <param name="hWnd">A handle to the window whose window procedure will receive the message.</param>
        /// <param name="msg">The message to be sent.</param>
        /// <param name="wParam">Additional message-specific information.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        ///     Brings the thread that created the specified window into the foreground and activates the window.
        ///     Keyboard input is directed to the window, and various visual cues are changed for the user.
        ///     The system assigns a slightly higher priority to the thread that created the foreground window than it does to
        ///     other threads.
        /// </summary>
        /// <param name="hWnd">A handle to the window that should be activated and brought to the foreground.</param>
        /// <returns>
        ///     If the window was brought to the foreground, the return value is nonzero.
        ///     If the window was not brought to the foreground, the return value is zero.
        /// </returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

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
        ///     Sets the show state and the restored, minimized, and maximized positions of the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="lpwndpl">
        ///     A pointer to the <see cref="WindowPlacement" /> structure that specifies the new show state and
        ///     window positions.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.
        ///     If the function fails, the return value is zero. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WindowPlacement lpwndpl);

        /// <summary>
        ///     Changes the text of the specified window's title bar (if it has one). If the specified window is a control, the
        ///     text of the control is changed.
        /// </summary>
        /// <param name="hwnd">A handle to the window or control whose text is to be changed.</param>
        /// <param name="lpString">The new title or control text.</param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.
        ///     If the function fails, the return value is zero. To get extended error information, call
        ///     <see cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(IntPtr hwnd, string lpString);

        /// <summary>
        ///     Sets the specified window's show state.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="nCmdShow">
        ///     Controls how the window is to be shown.
        ///     This parameter is ignored the first time an application calls ShowWindow, if the program that launched the
        ///     application provides a STARTUPINFO structure.
        ///     Otherwise, the first time ShowWindow is called, the value should be the value obtained by the WinMain function in
        ///     its nCmdShow parameter.
        ///     In subsequent calls, this parameter can be one of the following values.
        /// </param>
        /// <returns>
        ///     If the window was previously visible, the return value is nonzero.
        ///     If the window was previously hidden, the return value is zero.
        /// </returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, WindowStates nCmdShow);

        /// <summary>
        ///     Places (posts) a message in the message queue associated with the thread that created the specified window and
        ///     returns without waiting for the thread to process the message.
        ///     To post a message in the message queue associated with a thread, use the PostThreadMessage function.
        /// </summary>
        /// <param name="hWnd">
        ///     A handle to the window whose window procedure is to receive the message. The following values have
        ///     special meanings.
        /// </param>
        /// <param name="msg">The message to be posted.</param>
        /// <param name="wParam">Additional message-specific information.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.
        ///     If the function fails, the return value is zero.
        ///     To get extended error information, call <see cref="Marshal.GetLastWin32Error" />.
        ///     <see cref="Marshal.GetLastWin32Error" /> returns ERROR_NOT_ENOUGH_QUOTA when the limit is hit.
        /// </returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        ///     Retrieves a handle to the foreground window (the window with which the user is currently working).
        ///     The system assigns a slightly higher priority to the thread that creates the foreground window than it does to
        ///     other threads.
        /// </summary>
        /// <returns>
        ///     The return value is a handle to the foreground window. The foreground window can be NULL in certain
        ///     circumstances, such as when a window is losing activation.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true), SuppressUnmanagedCodeSecurity]
        public static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        internal static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(HookType code,
            HookProc func,
            IntPtr hInstance,
            int threadId);

        [DllImport("user32.dll", EntryPoint = "SetWindowsHookEx")]
        public static extern IntPtr SetWindowsHookLowLevel(HookType code,
            LowLevelProc func,
            IntPtr hInstance,
            int threadId);

        public static IntPtr SetWindowsHook(HookType hookType, LowLevelProc callback)
        {
            IntPtr hookId;
            using (var currentProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var currentModule = currentProcess.MainModule)
            {
                var handle = Kernel32.GetModuleHandle(currentModule.ModuleName);
                hookId = SetWindowsHookLowLevel(hookType, callback, handle, 0);
            }
            return hookId;
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        internal static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        internal static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr newValue);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        internal static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr newValue);

        [DllImport("user32.dll")]
        internal static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, int msg, int wParam,
            IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern int RegisterWindowMessage(string lpString);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

    }
}