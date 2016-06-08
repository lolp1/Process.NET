using System;

namespace Process.NET.Native
{
    /// <summary>
    ///     This delegate matches the type of parameter "lpfn" for the NativeMethods method "SetWindowsHookEx".
    ///     For more information: http://msdn.microsoft.com/en-us/library/ms644986(VS.85).aspx
    /// </summary>
    /// <param name="nCode"></param>
    /// <param name="wParam"></param>
    /// <param name="lParam"></param>
    /// <returns></returns>
    public delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);

    public delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
}