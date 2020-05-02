using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Process.NET.Applied;
using Process.NET.Native.Types;

namespace Process.NET.Windows.Keyboard
{
    public class KeyboardHook : IApplied
    {
        public delegate void KeyDownEventDelegate(KeyboardHookEventArgs e);

        public delegate void KeyUpEventDelegate(KeyboardHookEventArgs e);

        private IntPtr _hhook;

        private HookProc _hookproc;
        private bool _ispaused;
        public KeyDownEventDelegate KeyDownEvent = delegate { };
        public KeyUpEventDelegate KeyUpEvent = delegate { };

        public KeyboardHook(string name)
        {
            Identifier = name;
        }

        /// <summary>
        ///     When true, suspends firing of the hook notification events
        /// </summary>
        public bool IsPaused
        {
            get { return _ispaused; }
            set
            {
                if (value != _ispaused && value)
                    Disable();

                if (value != _ispaused && value == false)
                    Enable();

                _ispaused = value;
            }
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;
            IsDisposed = true;
            Disable();
            GC.SuppressFinalize(this);
        }

        public bool IsDisposed { get; set; }
        public bool MustBeDisposed { get; } = true;
        public string Identifier { get; }
        public bool IsEnabled { get; set; }

        public void Enable()
        {
            Trace.WriteLine($"Starting hook '{Identifier}'...", $"Hook.StartHook [{Thread.CurrentThread.Name}]");

            _hookproc = HookCallback;
            _hhook = SetWindowsHookEx(HookType.WhKeyboardLl, _hookproc,
                GetModuleHandle(System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName), 0);
            if (_hhook != null && _hhook != IntPtr.Zero)
            {
                IsEnabled = true;
                return;
            }

            var lastError = new Win32Exception(Marshal.GetLastWin32Error());
            Trace.TraceError(lastError.Message);
            IsDisposed = false;
        }

        public void Disable()
        {
            Trace.WriteLine($"Stopping hook '{Identifier}'...", $"Hook.StartHook [{Thread.CurrentThread.Name}]");

            UnhookWindowsHookEx(_hhook);
            IsEnabled = false;
        }

        private int HookCallback(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            var result = 0;

            try
            {
                if (!IsPaused && code >= 0)
                {
                    if (wParam.ToInt32() == WmSyskeydown || wParam.ToInt32() == WmKeydown)
                        KeyDownEvent(new KeyboardHookEventArgs(lParam));

                    if (wParam.ToInt32() == WmSyskeyup || wParam.ToInt32() == WmKeyup)
                        KeyUpEvent(new KeyboardHookEventArgs(lParam));
                }
            }
            finally
            {
                result = CallNextHookEx(IntPtr.Zero, code, wParam, ref lParam);
            }

            return result;
        }

        ~KeyboardHook()
        {
            if (MustBeDisposed)
                Dispose();
        }

        #region PInvoke

        private enum HookType
        {
            WhJournalrecord = 0,
            WhJournalplayback = 1,
            WhKeyboard = 2,
            WhGetmessage = 3,
            WhCallwndproc = 4,
            WhCbt = 5,
            WhSysmsgfilter = 6,
            WhMouse = 7,
            WhHardware = 8,
            WhDebug = 9,
            WhShell = 10,
            WhForegroundidle = 11,
            WhCallwndprocret = 12,
            WhKeyboardLl = 13,
            WhMouseLl = 14
        }

        private const int WmKeydown = 0x100;
        private const int WmKeyup = 0x101;

        private const int WmSyskeydown = 0x0104;
        public readonly int WmSyskeyup = 0x105;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        /// <summary>
        ///     Installs an application-defined hook procedure into a hook chain.
        /// </summary>
        /// <param name="idHook">The type of hook procedure to be installed.</param>
        /// <param name="lpfn">Reference to the hook callback method.</param>
        /// <param name="hMod">
        ///     A handle to the DLL containing the hook procedure pointed to by the lpfn parameter. The hMod
        ///     parameter must be set to NULL if the dwThreadId parameter specifies a thread created by the current process and if
        ///     the hook procedure is within the code associated with the current process.
        /// </param>
        /// <param name="dwThreadId">
        ///     The identifier of the thread with which the hook procedure is to be associated. If this
        ///     parameter is zero, the hook procedure is associated with all existing threads running in the same desktop as the
        ///     calling thread.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is the handle to the hook procedure. If the function fails, the
        ///     return value is NULL. To get extended error information, call GetLastError.
        /// </returns>
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(HookType idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

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
        private static extern int UnhookWindowsHookEx(IntPtr hhk);

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
        private static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        /// <summary>
        ///     An application-defined or library-defined callback function used with the SetWindowsHookEx function. The system
        ///     calls this function whenever an application calls the GetMessage or PeekMessage function and there is a keyboard
        ///     message (WM_KEYUP or WM_KEYDOWN) to be processed.
        /// </summary>
        /// <param name="code">
        ///     A code the hook procedure uses to determine how to process the message. If code is less than zero,
        ///     the hook procedure must pass the message to the CallNextHookEx function without further processing and should
        ///     return the value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">The virtual-key code of the key that generated the keystroke message.</param>
        /// <param name="lParam">
        ///     The repeat count, scan code, extended-key flag, context code, previous key-state flag, and
        ///     transition-state flag. For more information about the lParam parameter, see Keystroke Message Flags.
        /// </param>
        /// <returns>
        ///     If code is less than zero, the hook procedure must return the value returned by CallNextHookEx. If code is
        ///     greater than or equal to zero, and the hook procedure did not process the message, it is highly recommended that
        ///     you call CallNextHookEx and return the value it returns; otherwise bad stuff.
        /// </returns>
        private delegate int HookProc(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        #endregion
    }
}