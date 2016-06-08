using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Process.NET.Native.Types;
using Process.NET.Utilities;

namespace Process.NET.Windows.Keyboard
{
    /// <summary>
    ///     Class defining a virtual keyboard using the API Message.
    /// </summary>
    public class MessageKeyboard : IKeyboard
    {
        protected static readonly List<Tuple<IntPtr, Keys>> PressedKeys = new List<Tuple<IntPtr, Keys>>();

        public MessageKeyboard(IWindow window)
        {
            Window = window;
        }

        protected IWindow Window { get; set; }

        /// <summary>
        ///     Presses the specified virtual key to the window at a specified interval.
        /// </summary>
        /// <param name="key">The virtual key to press.</param>
        /// <param name="interval">The interval between the key activations.</param>
        public void Press(Keys key, TimeSpan interval)
        {
            // Create the tuple
            var tuple = Tuple.Create(Window.Handle, key);

            // If the key is already pressed
            if (PressedKeys.Contains(tuple))
                return;

            // Add the key to the collection
            PressedKeys.Add(tuple);
            // Start a new task to press the key at the specified interval
            Task.Run(async () =>
            {
                // While the key must be pressed
                while (PressedKeys.Contains(tuple))
                {
                    // Press the key
                    Press(key);
                    // Wait the interval
                    await Task.Delay(interval);
                }
            });
        }

        /// <summary>
        ///     Presses and releaes the specified virtual key to the window.
        /// </summary>
        /// <param name="key">The virtual key to press and release.</param>
        public void PressRelease(Keys key)
        {
            Press(key);
            Thread.Sleep(10);
            Release(key);
        }

        /// <summary>
        ///     Writes the text representation of the specified array of objects to the window using the specified format
        ///     information.
        /// </summary>
        /// <param name="text">A composite format string.</param>
        /// <param name="args">An array of objects to write using format.</param>
        public void Write(string text, params object[] args)
        {
            foreach (var character in string.Format(text, args))
                Write(character);
        }

        /// <summary>
        ///     Presses the specified virtual key to the window.
        /// </summary>
        /// <param name="key">The virtual key to press.</param>
        public void Press(Keys key)
        {
            Window.PostMessage(WindowsMessages.KeyDown, new IntPtr((int) key), MakeKeyParameter(key, false));
        }

        /// <summary>
        ///     Writes the specified character to the window.
        /// </summary>
        /// <param name="character">The character to write.</param>
        public void Write(char character)
        {
            Window.PostMessage(WindowsMessages.Char, new IntPtr(character), IntPtr.Zero);
        }

        /// <summary>
        ///     Releases the specified virtual key to the window.
        /// </summary>
        /// <param name="key">The virtual key to release.</param>
        public virtual void Release(Keys key)
        {
            // Create the tuple
            var tuple = Tuple.Create(Window.Handle, key);

            // If the key is pressed with an interval
            if (PressedKeys.Contains(tuple))
                PressedKeys.Remove(tuple);
            Window.PostMessage(WindowsMessages.KeyUp, new IntPtr((int) key), MakeKeyParameter(key, true));
        }

        /// <summary>
        ///     Makes the lParam for a key depending on several settings.
        /// </summary>
        /// <param name="key">
        ///     [16-23 bits] The virtual key.
        /// </param>
        /// <param name="keyUp">
        ///     [31 bit] The transition state.
        ///     The value is always 0 for a <see cref="WindowsMessages.KeyDown" /> message.
        ///     The value is always 1 for a <see cref="WindowsMessages.KeyUp" /> message.
        /// </param>
        /// <param name="fRepeat">
        ///     [30 bit] The previous key state.
        ///     The value is 1 if the key is down before the message is sent, or it is zero if the key is up.
        ///     The value is always 1 for a <see cref="WindowsMessages.KeyUp" /> message.
        /// </param>
        /// <param name="cRepeat">
        ///     [0-15 bits] The repeat count for the current message.
        ///     The value is the number of times the keystroke is autorepeated as a result of the user holding down the key.
        ///     If the keystroke is held long enough, multiple messages are sent. However, the repeat count is not cumulative.
        ///     The repeat count is always 1 for a <see cref="WindowsMessages.KeyUp" /> message.
        /// </param>
        /// <param name="altDown">
        ///     [29 bit] The context code.
        ///     The value is always 0 for a <see cref="WindowsMessages.KeyDown" /> message.
        ///     The value is always 0 for a <see cref="WindowsMessages.KeyUp" /> message.
        /// </param>
        /// <param name="fExtended">
        ///     [24 bit] Indicates whether the key is an extended key, such as the right-hand ALT and CTRL keys that appear on
        ///     an enhanced 101- or 102-key keyboard. The value is 1 if it is an extended key; otherwise, it is 0.
        /// </param>
        /// <returns>The return value is the lParam when posting or sending a message regarding key press.</returns>
        /// <remarks>
        ///     KeyDown resources: http://msdn.microsoft.com/en-us/library/windows/desktop/ms646280%28v=vs.85%29.aspx
        ///     KeyUp resources:  http://msdn.microsoft.com/en-us/library/windows/desktop/ms646281%28v=vs.85%29.aspx
        /// </remarks>
        private static IntPtr MakeKeyParameter(Keys key, bool keyUp, bool fRepeat, int cRepeat, bool altDown,
            bool fExtended)
        {
            // Create the result and assign it with the repeat count
            var result = (uint)cRepeat;
            // Add the scan code with a left shift operation
            result |= (uint)WindowHelper.MapVirtualKey(key, TranslationTypes.VirtualKeyToScanCode) << 16;
            // Does we need to set the extended flag ?
            if (fExtended)
            {
                result |= 0x1000000;
            }
            // Does we need to set the alt flag ?
            if (altDown)
            {
                result |= 0x20000000;
            }
            // Does we need to set the repeat flag ?
            if (fRepeat)
            {
                result |= 0x40000000;
            }

            // Does we need to set the keyUp flag ?
            if (keyUp)
            {
                result |= 0x80000000;
            }

            return new IntPtr(result);
        }

        /// <summary>
        ///     Makes the lParam for a key depending on several settings.
        /// </summary>
        /// <param name="key">The virtual key.</param>
        /// <param name="keyUp">
        ///     The transition state.
        ///     The value is always 0 for a <see cref="WindowsMessages.KeyDown" /> message.
        ///     The value is always 1 for a <see cref="WindowsMessages.KeyUp" /> message.
        /// </param>
        /// <returns>The return value is the lParam when posting or sending a message regarding key press.</returns>
        private static IntPtr MakeKeyParameter(Keys key, bool keyUp)
        {
            return MakeKeyParameter(key, keyUp, keyUp, 1, false, false);
        }
    }
}