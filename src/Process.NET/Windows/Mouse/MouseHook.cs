using System;
using System.Runtime.InteropServices;
using Process.NET.Native;
using Process.NET.Native.Types;

namespace Process.NET.Windows.Mouse
{
    public enum HookEventType
    {
        Keyboard,
        Mouse
    }

    public sealed class MouseHook : IDisposable
    {
        private readonly LowLevelProc _callback;

        private IntPtr _hookId;

        public MouseHook()
        {
            _callback = MouseHookCallback;
        }

        public bool IsEnabled { get; private set; }

        public bool IsDisposed { get; set; }
        public bool MustBeDisposed { get; set; } = true;
        public string Identifier { get; set; }

        public void Enable()
        {
            _hookId = User32.SetWindowsHook(HookType.WH_MOUSE_LL, _callback);
            IsEnabled = true;
        }

        public void Disable()
        {
            if (!IsEnabled) return;
            User32.UnhookWindowsHookEx(_hookId);
            IsEnabled = false;
        }

        /// <summary>
        ///     This is the callback method that is called whenever a low level mouse event is triggered.
        ///     We use it to call our individual custom events.
        /// </summary>
        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var lParamStruct = (MSLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof (MSLLHOOKSTRUCT));
                var e = new MouseHookEventArgs(lParamStruct);
                switch ((MouseMessages) wParam)
                {
                    case MouseMessages.WmMouseMove:
                        TriggerMouseEvent(e, MouseEventNames.MouseMove, OnMove);
                        break;
                    case MouseMessages.WmLButtonDown:
                        TriggerMouseEvent(e, MouseEventNames.LeftButtonDown, OnLeftButtonDown);
                        break;
                    case MouseMessages.WmLButtonUp:
                        TriggerMouseEvent(e, MouseEventNames.LeftButtonUp, OnLeftButtonUp);
                        break;
                    case MouseMessages.WmRButtonDown:
                        TriggerMouseEvent(e, MouseEventNames.RightButtonDown, OnRightButtonDown);
                        break;
                    case MouseMessages.WmRButtonUp:
                        TriggerMouseEvent(e, MouseEventNames.RightButtonUp, OnRightButtonUp);
                        break;
                    case MouseMessages.WmMButtonDown:
                        TriggerMouseEvent(e, MouseEventNames.MiddleButtonDown, OnMiddleButtonDown);
                        break;
                    case MouseMessages.WmMButtonUp:
                        TriggerMouseEvent(e, MouseEventNames.MouseMove, OnMove);
                        e.MouseEventName = MouseEventNames.MiddleButtonUp;
                        OnMiddleButtonUp(e);
                        break;
                    case MouseMessages.WmMouseWheel:
                        TriggerMouseEvent(e, MouseEventNames.MouseMove, OnMove);
                        e.MouseEventName = MouseEventNames.MouseWheel;
                        OnWheel(e);
                        break;
                }
            }
            return (IntPtr) User32.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private static void TriggerMouseEvent(MouseHookEventArgs e, MouseEventNames name,
            Action<MouseHookEventArgs> method)
        {
            e.MouseEventName = name;
            method(e);
        }

        #region Custom Events

        public event EventHandler<MouseHookEventArgs> Move;

        private void OnMove(MouseHookEventArgs e)
        {
            Move?.Invoke(this, e);
            OnMouseEvent(e);
        }

        public event EventHandler<MouseHookEventArgs> LeftButtonDown;

        private void OnLeftButtonDown(MouseHookEventArgs e)
        {
            LeftButtonDown?.Invoke(this, e);
            OnMouseEvent(e);
        }

        public event EventHandler<MouseHookEventArgs> LeftButtonUp;

        private void OnLeftButtonUp(MouseHookEventArgs e)
        {
            LeftButtonUp?.Invoke(this, e);
            OnMouseEvent(e);
        }

        public event EventHandler<MouseHookEventArgs> RightButtonDown;

        private void OnRightButtonDown(MouseHookEventArgs e)
        {
            RightButtonDown?.Invoke(this, e);
            OnMouseEvent(e);
        }

        public event EventHandler<MouseHookEventArgs> RightButtonUp;

        private void OnRightButtonUp(MouseHookEventArgs e)
        {
            RightButtonUp?.Invoke(this, e);
            OnMouseEvent(e);
        }

        public event EventHandler<MouseHookEventArgs> MiddleButtonDown;

        private void OnMiddleButtonDown(MouseHookEventArgs e)
        {
            MiddleButtonDown?.Invoke(this, e);
            OnMouseEvent(e);
        }

        public event EventHandler<MouseHookEventArgs> MiddleButtonUp;

        private void OnMiddleButtonUp(MouseHookEventArgs e)
        {
            MiddleButtonUp?.Invoke(this, e);
            OnMouseEvent(e);
        }

        public event EventHandler<MouseHookEventArgs> Wheel;

        private void OnWheel(MouseHookEventArgs e)
        {
            Wheel?.Invoke(this, e);
            OnMouseEvent(e);
        }

        public event EventHandler<MouseHookEventArgs> MouseEvent;

        private void OnMouseEvent(MouseHookEventArgs e)
        {
            MouseEvent?.Invoke(this, e);
        }

        #endregion

        #region IDisposable Members / Finalizer

        /// <summary>
        ///     Call this method to unhook the Mouse Hook, and to release resources allocated to it.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;
            IsDisposed = true;
            if (IsEnabled)
                Disable();
            GC.SuppressFinalize(this);
        }

        ~MouseHook()
        {
            if (MustBeDisposed)
                Disable();
        }

        #endregion
    }
}