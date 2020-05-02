using System;
using Process.NET.Native.Types;

namespace Process.NET.Windows.Keyboard
{
    /// <summary>
    ///     Class defining a virtual keyboard.
    /// </summary>
    public interface IKeyboard
    {
        /// <summary>
        ///     Presses the specified virtual key to the window at a specified interval.
        /// </summary>
        /// <param name="key">The virtual key to press.</param>
        /// <param name="interval">The interval between the key activations.</param>
        void Press(Keys key, TimeSpan interval);

        /// <summary>
        ///     Presses and releaes the specified virtual key to the window.
        /// </summary>
        /// <param name="key">The virtual key to press and release.</param>
        void PressRelease(Keys key);

        /// <summary>
        ///     Writes the text representation of the specified array of objects to the window using the specified format
        ///     information.
        /// </summary>
        /// <param name="text">A composite format string.</param>
        /// <param name="args">An array of objects to write using format.</param>
        void Write(string text, params object[] args);

        /// <summary>
        ///     Presses the specified virtual key to the window.
        /// </summary>
        /// <param name="key">The virtual key to press.</param>
        void Press(Keys key);

        /// <summary>
        ///     Writes the specified character to the window.
        /// </summary>
        /// <param name="character">The character to write.</param>
        void Write(char character);
    }
}