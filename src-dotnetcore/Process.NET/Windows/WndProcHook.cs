using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using Process.NET.Applied;
using Process.NET.Native;
using Process.NET.Native.Types;

namespace Process.NET.Windows
{
    public delegate IntPtr WindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    public abstract class WindowProcHook : IApplied
    {
        // ReSharper disable once InconsistentNaming
        private const int GWL_WNDPROC = -4;
        private WindowProc _newCallback;

        private IntPtr _oldCallback;

        protected WindowProcHook(IntPtr handle, string identifier = "")
        {
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = "WindowProcHook - " + handle.ToString("X");
            }

            Identifier = identifier;
            Handle = handle;
        }

        protected WindowProcHook(string procName, int index = 0)
        {
            var processesByName = System.Diagnostics.Process.GetProcessesByName(procName).ToList();

            if (processesByName == null)
            {
                throw new NullReferenceException($"Could not find a process by the name of {procName}");
            }

            var process = index == 0 ? processesByName.First() : processesByName[index];

            Identifier = $"WndProc hook - {process.MainWindowTitle}";
            Handle = process.MainWindowHandle;
        }

        protected IntPtr Handle { get; set; }

        public void Enable()
        {
            _newCallback = OnWndProc;

            _oldCallback = Kernel32.SetWindowLongPtr(Handle, GWL_WNDPROC,
                Marshal.GetFunctionPointerForDelegate(_newCallback));

            if (_oldCallback == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            IsEnabled = true;
        }

        public string Identifier { get; }

        public bool IsDisposed { get; protected set; }

        public bool IsEnabled { get; protected set; }

        public bool MustBeDisposed { get; set; } = true;

        public void Disable()
        {
            if (_newCallback == null)
            {
                return;
            }

            Kernel32.SetWindowLongPtr(Handle, GWL_WNDPROC, _oldCallback);
            _newCallback = null;
            IsEnabled = false;
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            if (IsEnabled)
            {
                Disable();
            }

            GC.SuppressFinalize(this);
        }

        protected virtual IntPtr OnWndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            return Kernel32.CallWindowProc(_oldCallback, hWnd, msg, wParam, lParam);
        }

        ~WindowProcHook()
        {
            if (MustBeDisposed)
            {
                Dispose();
            }
        }

        public override string ToString()
        {
            return Identifier;
        }
    }
}