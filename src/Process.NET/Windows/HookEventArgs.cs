using System;
using Process.NET.Windows.Mouse;

namespace Process.NET.Windows
{
    public abstract class HookEventArgs : EventArgs
    {
        protected HookEventType EventType { get; set; }
    }
}