using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using Process.NET.Applied;
using Process.NET.Extensions;
using Process.NET.Marshaling;
using Process.NET.Native;
using Process.NET.Utilities;

namespace Process.NET.Windows
{
    public delegate IntPtr WindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    public abstract class WndProcHook : IApplied
    {
        // ReSharper disable once InconsistentNaming
        private const int GWL_WNDPROC = -4;
        private WindowProc _newCallback;

        private IntPtr _oldCallback;
        private bool _isEnabled;

        protected WndProcHook(IntPtr handle, string identifier = "")
        {
            if (string.IsNullOrEmpty(identifier))
                identifier = "WindowProcHook - " + handle.ToString("X");

            Identifier = identifier;
            Handle = handle;
        }

        protected WndProcHook(IWindow window) : this(window.Handle, window.Title)
        {
        }

        protected WndProcHook(IProcess window) : this(window.Native.MainWindowHandle, window.WindowFactory.MainWindow.Title)
        {
        }

        protected WndProcHook(string processName) : this(ProcessHelper.GetMainWindowHandle(processName))
        {
        }

        public IntPtr Handle { get; protected set; }

        public void Enable()
        {
            _newCallback = WndProc;

            _oldCallback = Kernel32.SetWindowLongPtr(Handle, GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(_newCallback));

            if (_oldCallback == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            IsEnabled = true;
        }

        public string Identifier { get; }

        public bool IsDisposed { get; protected set; }

        public bool IsEnabled { get; set; }

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

        public event EventHandler<Message> MessageReceived;

        protected virtual IntPtr OnWndProc(ref Message msg)
        {
            return Kernel32.CallWindowProc(_oldCallback, msg.HWnd, msg.Msg, msg.WParam, msg.LParam);
        }

        private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            var message = Message.Create(hWnd, msg, wParam, lParam);

            return OnWndProc(ref message);
        }

        public void SendMessage(Message msg)
        {
            User32.SendMessage(Handle, msg.Msg, msg.WParam, msg.LParam);
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

        protected virtual void OnMessageReceived(ref Message e)
        {
            MessageReceived?.Invoke(this, e);
        }
    }
}