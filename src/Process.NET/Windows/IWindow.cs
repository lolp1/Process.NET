using System;
using System.Collections.Generic;
using Process.NET.Native.Types;
using Process.NET.Threads;
using Process.NET.Windows.Keyboard;
using Process.NET.Windows.Mouse;

namespace Process.NET.Windows
{
    public interface IWindow : IDisposable
    {
        IEnumerable<IWindow> Children { get; }
        string ClassName { get; }
        IntPtr Handle { get; }
        int Height { get; set; }
        bool IsActivated { get; }
        bool IsMainWindow { get; }
        IKeyboard Keyboard { get; set; }
        IMouse Mouse { get; set; }
        WindowPlacement Placement { get; set; }
        WindowStates State { get; set; }
        IRemoteThread Thread { get; }
        string Title { get; set; }
        int Width { get; set; }
        int X { get; set; }
        int Y { get; set; }

        void Activate();
        void Close();
        void Flash();
        void Flash(uint count, TimeSpan timeout, FlashWindowFlags flags = FlashWindowFlags.All);
        void PostMessage(WindowsMessages message, UIntPtr wParam, UIntPtr lParam);
        void PostMessage(uint message, UIntPtr wParam, UIntPtr lParam);
        IntPtr SendMessage(WindowsMessages message, UIntPtr wParam, IntPtr lParam);
        IntPtr SendMessage(uint message, UIntPtr wParam, IntPtr lParam);
    }
}