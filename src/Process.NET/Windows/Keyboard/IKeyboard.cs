using System;
using Process.NET.Native.Types;

namespace Process.NET.Windows.Keyboard
{
    public interface IKeyboard
    {
        void Press(Keys key);
        void Press(Keys key, TimeSpan interval);
        void PressRelease(Keys key);
        void Release(Keys key);
        void Write(char character);
        void Write(string text, params object[] args);
    }
}