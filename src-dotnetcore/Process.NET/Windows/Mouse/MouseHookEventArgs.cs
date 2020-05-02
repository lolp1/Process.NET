using Process.NET.Native.Types;

namespace Process.NET.Windows.Mouse
{
    public class MouseHookEventArgs : HookEventArgs
    {
        public MouseHookEventArgs(MSLLHOOKSTRUCT lparam)
        {
            EventType = HookEventType.Mouse;
            LParam = lparam;
        }

        private MSLLHOOKSTRUCT LParam { get; }

        public Point Position => LParam.Point;

        public MouseScrollDirection ScrollDirection
        {
            get
            {
                if (MouseEventName != MouseEventNames.MouseWheel)
                    return MouseScrollDirection.None;
                return LParam.MouseData >> 16 > 0 ? MouseScrollDirection.Up : MouseScrollDirection.Down;
            }
        }

        public MouseEventNames MouseEventName { get; internal set; }
    }
}