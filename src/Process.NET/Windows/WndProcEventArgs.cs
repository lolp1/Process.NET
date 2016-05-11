using System;

namespace Process.NET.Windows
{
    public class WndProcEventArgs : EventArgs
    {
        public WndProcEventArgs(IntPtr hwnd, int msg, int wParam, IntPtr lParam)
        {
            Hwnd = hwnd;
            Msg = msg;
            WParam = wParam;
            LParam = lParam;
        }

        public IntPtr Hwnd { get; }

        public int Msg { get; }

        public int WParam { get; }

        public IntPtr LParam { get; }
    }
}