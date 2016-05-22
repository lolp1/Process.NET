using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Process.NET.Applied;
using Process.NET.Native;

namespace Process.NET.Windows
{
    public delegate IntPtr WindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    public abstract class WndProcHook : IApplied
    {
        // ReSharper disable once InconsistentNaming
        private const int GWL_WNDPROC = -4;
        private WindowProc _newCallback;

        private IntPtr _oldCallback;

        protected WndProcHook(IntPtr handle, string identifier = "")
        {
            if (string.IsNullOrEmpty(identifier))
                identifier = "WindowProcHook - " + handle.ToString("X");

            Identifier = identifier;
            Handle = handle;
        }

        protected IntPtr Handle { get; set; }

        public void Enable()
        {
            _newCallback = WndProc;

            IntPtr fptr = Marshal.GetFunctionPointerForDelegate(_newCallback);

            _oldCallback = Kernel32.SetWindowLongPtr(Handle, GWL_WNDPROC, fptr);

            if (_oldCallback == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            IsEnabled = true;
        }

        public string Identifier { get; }

        public bool IsDisposed { get; protected set; }

        public bool IsEnabled { get; protected set; }

        public bool MustBeDisposed { get; set; } = true;

        public void Disable()
        {
            if (_newCallback == null)
                return;

            Kernel32.SetWindowLongPtr(Handle, GWL_WNDPROC, _oldCallback);
            _newCallback = null;
            IsEnabled = false;
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            if (IsEnabled)
                Disable();
            GC.SuppressFinalize(this);
        }

        public event EventHandler<WndProcEventArgs> OnWndProc;

        protected virtual IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            return Kernel32.CallWindowProc(_oldCallback, hWnd, msg, wParam, lParam);
        }

        public void SendMessage(int msg, IntPtr wparam)
        {
            User32.SendMessage(Handle, msg, wparam, IntPtr.Zero);
        }

        ~WndProcHook()
        {
            if (MustBeDisposed)
                Dispose();
        }

        public override string ToString()
        {
            return Identifier;
        }

        protected virtual void ProcessWndProc(WndProcEventArgs e)
        {
            OnWndProc?.Invoke(this, e);
        }
    }
}