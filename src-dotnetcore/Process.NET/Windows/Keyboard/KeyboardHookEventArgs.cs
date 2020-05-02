using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using Process.NET.Native.Types;

namespace Process.NET.Windows.Keyboard
{
    public class KeyboardHookEventArgs
    {
        public KeyboardHookEventArgs(KBDLLHOOKSTRUCT lParam)
        {
            Key = (Keys) lParam.vkCode;

            //Control.ModifierKeys doesn't capture alt/win, and doesn't have r/l granularity
            IsLAltPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_LALT) & KEY_PRESSED) || Key == Keys.LMenu;
            IsRAltPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_RALT) & KEY_PRESSED) || Key == Keys.RMenu;

            IsLCtrlPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_LCONTROL) & KEY_PRESSED) ||
                             Key == Keys.LControlKey;
            IsRCtrlPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_RCONTROL) & KEY_PRESSED) ||
                             Key == Keys.RControlKey;

            IsLShiftPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_LSHIFT) & KEY_PRESSED) ||
                              Key == Keys.LShiftKey;
            IsRShiftPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_RSHIFT) & KEY_PRESSED) ||
                              Key == Keys.RShiftKey;

            IsLWinPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_LWIN) & KEY_PRESSED) || Key == Keys.LWin;
            IsRWinPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_RWIN) & KEY_PRESSED) || Key == Keys.RWin;

            if (
                new[]
                {
                    Keys.LMenu, Keys.RMenu, Keys.LControlKey, Keys.RControlKey, Keys.LShiftKey, Keys.RShiftKey,
                    Keys.LWin,
                    Keys.RWin
                }.Contains(Key))
                Key = Keys.None;
        }

        public Keys Key { get; }

        public bool IsAltPressed => IsLAltPressed || IsRAltPressed;
        public bool IsLAltPressed { get; }
        public bool IsRAltPressed { get; }

        public bool IsCtrlPressed => IsLCtrlPressed || IsRCtrlPressed;
        public bool IsLCtrlPressed { get; }
        public bool IsRCtrlPressed { get; }

        public bool IsShiftPressed => IsLShiftPressed || IsRShiftPressed;
        public bool IsLShiftPressed { get; }
        public bool IsRShiftPressed { get; }

        public bool IsWinPressed => IsLWinPressed || IsRWinPressed;
        public bool IsLWinPressed { get; }
        public bool IsRWinPressed { get; }

        public override string ToString()
        {
            return $"Key={Key}; Win={IsWinPressed}; Alt={IsAltPressed}; Ctrl={IsCtrlPressed}; Shift={IsShiftPressed}";
        }

        #region PInvoke

        [DllImport("user32.dll")]
        private static extern short GetKeyState(VirtualKeyStates nVirtKey);

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private enum VirtualKeyStates
        {
            VK_LWIN = 0x5B,
            VK_RWIN = 0x5C,
            VK_LSHIFT = 0xA0,
            VK_RSHIFT = 0xA1,
            VK_LCONTROL = 0xA2,
            VK_RCONTROL = 0xA3,
            VK_LALT = 0xA4, //aka VK_LMENU
            VK_RALT = 0xA5 //aka VK_RMENU
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")] private const int KEY_PRESSED = 0x8000;

        #endregion
    }
}