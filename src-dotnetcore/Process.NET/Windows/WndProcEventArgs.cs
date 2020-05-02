using System;

namespace Process.NET.Windows
{
    public class WndProcEventArgs : EventArgs
    {
        public WndProcEventArgs(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            Hwnd = hwnd;
            Msg = msg;
            WParam = wParam;
            LParam = lParam;
        }

        public IntPtr Hwnd { get; }

        public int Msg { get; }

        public IntPtr WParam { get; }

        public IntPtr LParam { get; }
    }
}