using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Process.NET.Marshaling;
using Process.NET.Native;
using Process.NET.Native.Types;
using Process.NET.Windows;

namespace Process.NET.Utilities
{
    /// <summary>
    ///     Static core class providing tools for managing windows.
    /// </summary>
    public static class WindowHelper
    {
        /// <summary>
        ///     Retrieves the name of the class to which the specified window belongs.
        /// </summary>
        /// <param name="windowHandle">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <returns>The return values is the class name string.</returns>
        public static string GetClassName(IntPtr windowHandle)
        {
            // Check if the handle is valid
            HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

            // Get the window class name
            var stringBuilder = new StringBuilder(char.MaxValue);
            if (User32.GetClassName(windowHandle, stringBuilder, stringBuilder.Capacity) == 0)
                throw new Win32Exception("Couldn't get the class name of the window or the window has no class name.");

            return stringBuilder.ToString();
        }

        /// <summary>
        ///     Retrieves a handle to the foreground window (the window with which the user is currently working).
        /// </summary>
        /// <returns>
        ///     A handle to the foreground window. The foreground window can be <c>IntPtr.Zero</c> in certain circumstances,
        ///     such as when a window is losing activation.
        /// </returns>
        public static IntPtr GetForegroundWindow()
        {
            return User32.GetForegroundWindow();
        }

        /// <summary>
        ///     Retrieves the specified system metric or system configuration setting.
        /// </summary>
        /// <param name="metric">The system metric or configuration setting to be retrieved.</param>
        /// <returns>The return value is the requested system metric or configuration setting.</returns>
        public static int GetSystemMetrics(SystemMetrics metric)
        {
            var ret = User32.GetSystemMetrics(metric);

            if (ret != 0)
                return ret;

            throw new Win32Exception(
                "The call of GetSystemMetrics failed. Unfortunately, GetLastError code doesn't provide more information.");
        }

        /// <summary>
        ///     Gets the text of the specified window's title bar.
        /// </summary>
        /// <param name="windowHandle">A handle to the window containing the text.</param>
        /// <returns>The return value is the window's title bar.</returns>
        public static string GetWindowText(IntPtr windowHandle)
        {
            // Check if the handle is valid
            HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

            // Get the size of the window's title
            var capacity = User32.GetWindowTextLength(windowHandle);
            // If the window doesn't contain any title
            if (capacity == 0)
                return string.Empty;

            // Get the text of the window's title bar text
            var stringBuilder = new StringBuilder(capacity + 1);
            if (User32.GetWindowText(windowHandle, stringBuilder, stringBuilder.Capacity) == 0)
                throw new Win32Exception("Couldn't get the text of the window's title bar or the window has no title.");

            return stringBuilder.ToString();
        }

        /// <summary>
        ///     Retrieves the show state and the restored, minimized, and maximized positions of the specified window.
        /// </summary>
        /// <param name="windowHandle">A handle to the window.</param>
        /// <returns>
        ///     The return value is a <see cref="WindowPlacement" /> structure that receives the show state and position
        ///     information.
        /// </returns>
        public static WindowPlacement GetWindowPlacement(IntPtr windowHandle)
        {
            // Check if the handle is valid
            HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

            // Allocate a WindowPlacement structure
            WindowPlacement placement;
            placement.Length = Marshal.SizeOf(typeof (WindowPlacement));

            // Get the window placement
            if (!User32.GetWindowPlacement(windowHandle, out placement))
                throw new Win32Exception("Couldn't get the window placement.");

            return placement;
        }

        /// <summary>
        ///     Retrieves the identifier of the process that created the window.
        /// </summary>
        /// <param name="windowHandle">A handle to the window.</param>
        /// <returns>The return value is the identifier of the process that created the window.</returns>
        public static int GetWindowProcessId(IntPtr windowHandle)
        {
            // Check if the handle is valid
            HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

            // Get the process id
            int processId;
            User32.GetWindowThreadProcessId(windowHandle, out processId);

            return processId;
        }

        /// <summary>
        ///     Retrieves the identifier of the thread that created the specified window.
        /// </summary>
        /// <param name="windowHandle">A handle to the window.</param>
        /// <returns>The return value is the identifier of the thread that created the window.</returns>
        public static int GetWindowThreadId(IntPtr windowHandle)
        {
            // Check if the handle is valid
            HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

            // Get the thread id
            int trash;
            return User32.GetWindowThreadProcessId(windowHandle, out trash);
        }

        /// <summary>
        ///     Enumerates all the windows on the screen.
        /// </summary>
        /// <returns>A collection of handles of all the windows.</returns>
        public static IEnumerable<IntPtr> EnumAllWindows()
        {
            // Create the list of windows
            var list = new List<IntPtr>();

            // For each top-level windows
            foreach (var topWindow in EnumTopLevelWindows())
            {
                // Add this window to the list
                list.Add(topWindow);
                // Enumerate and add the children of this window
                list.AddRange(EnumChildWindows(topWindow));
            }

            // Return the list of windows
            return list;
        }

        /// <summary>
        ///     Enumerates recursively all the child windows that belong to the specified parent window.
        /// </summary>
        /// <param name="parentHandle">The parent window handle.</param>
        /// <returns>A collection of handles of the child windows.</returns>
        public static IEnumerable<IntPtr> EnumChildWindows(IntPtr parentHandle)
        {
            // Create the list of windows
            var list = new List<IntPtr>();
            // Create the callback
            EnumWindowsProc callback = delegate(IntPtr windowHandle, IntPtr lParam)
            {
                list.Add(windowHandle);
                return true;
            };
            // Enumerate all windows
            User32.EnumChildWindows(parentHandle, callback, IntPtr.Zero);

            // Returns the list of the windows
            return list.ToArray();
        }

        /// <summary>
        ///     Enumerates all top-level windows on the screen. This function does not search child windows.
        /// </summary>
        /// <returns>A collection of handles of top-level windows.</returns>
        public static IEnumerable<IntPtr> EnumTopLevelWindows()
        {
            // When passing a null pointer, this function is equivalent to EnumWindows
            return EnumChildWindows(IntPtr.Zero);
        }

        /// <summary>
        ///     Flashes the specified window one time. It does not change the active state of the window.
        ///     To flash the window a specified number of times, use the
        ///     <see cref="FlashWindowEx(IntPtr, FlashWindowFlags, int, TimeSpan)" /> function.
        /// </summary>
        /// <param name="windowHandle">A handle to the window to be flashed. The window can be either open or minimized.</param>
        /// <returns>
        ///     The return value specifies the window's state before the call to the <see cref="FlashWindow" /> function.
        ///     If the window caption was drawn as active before the call, the return value is nonzero. Otherwise, the return value
        ///     is zero.
        /// </returns>
        public static bool FlashWindow(IntPtr windowHandle)
        {
            // Check if the handle is valid
            HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

            // Flash the window
            return User32.FlashWindow(windowHandle, true);
        }

        /// <summary>
        ///     Flashes the specified window. It does not change the active state of the window.
        /// </summary>
        /// <param name="windowHandle">A handle to the window to be flashed. The window can be either opened or minimized.</param>
        /// <param name="flags">The flash status.</param>
        /// <param name="count">The number of times to flash the window.</param>
        /// <param name="timeout">The rate at which the window is to be flashed.</param>
        public static void FlashWindowEx(IntPtr windowHandle, FlashWindowFlags flags, int count, TimeSpan timeout)
        {
            // Check if the handle is valid
            HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

            // Create the data structure
            var flashInfo = new FlashInfo
            {
                Size = Marshal.SizeOf(typeof (FlashInfo)),
                Hwnd = windowHandle,
                Flags = flags,
                Count = count,
                Timeout = Convert.ToInt32(timeout.TotalMilliseconds)
            };

            // Flash the window
            User32.FlashWindowEx(ref flashInfo);
        }

        /// <summary>
        ///     Flashes the specified window. It does not change the active state of the window. The function uses the default
        ///     cursor blink rate.
        /// </summary>
        /// <param name="windowHandle">A handle to the window to be flashed. The window can be either opened or minimized.</param>
        /// <param name="flags">The flash status.</param>
        /// <param name="count">The number of times to flash the window.</param>
        public static void FlashWindowEx(IntPtr windowHandle, FlashWindowFlags flags, int count)
        {
            FlashWindowEx(windowHandle, flags, count, TimeSpan.FromMilliseconds(0));
        }

        /// <summary>
        ///     Flashes the specified window. It does not change the active state of the window. The function uses the default
        ///     cursor blink rate.
        /// </summary>
        /// <param name="windowHandle">A handle to the window to be flashed. The window can be either opened or minimized.</param>
        /// <param name="flags">The flash status.</param>
        public static void FlashWindowEx(IntPtr windowHandle, FlashWindowFlags flags)
        {
            FlashWindowEx(windowHandle, flags, 0);
        }

        /// <summary>
        ///     Translates (maps) a virtual-key code into a scan code or character value, or translates a scan code into a
        ///     virtual-key code.
        ///     To specify a handle to the keyboard layout to use for translating the specified code, use the MapVirtualKeyEx
        ///     function.
        /// </summary>
        /// <param name="key">
        ///     The virtual key code or scan code for a key. How this value is interpreted depends on the value of the uMapType
        ///     parameter.
        /// </param>
        /// <param name="translation">
        ///     The translation to be performed. The value of this parameter depends on the value of the uCode parameter.
        /// </param>
        /// <returns>
        ///     The return value is either a scan code, a virtual-key code, or a character value, depending on the value of uCode
        ///     and uMapType.
        ///     If there is no translation, the return value is zero.
        /// </returns>
        public static int MapVirtualKey(int key, TranslationTypes translation)
        {
            return User32.MapVirtualKey(key, translation);
        }

        /// <summary>
        ///     Translates (maps) a virtual-key code into a scan code or character value, or translates a scan code into a
        ///     virtual-key code.
        ///     To specify a handle to the keyboard layout to use for translating the specified code, use the MapVirtualKeyEx
        ///     function.
        /// </summary>
        /// <param name="key">
        ///     The virtual key code for a key. How this value is interpreted depends on the value of the uMapType parameter.
        /// </param>
        /// <param name="translation">
        ///     The translation to be performed. The value of this parameter depends on the value of the uCode parameter.
        /// </param>
        /// <returns>
        ///     The return value is either a scan code, a virtual-key code, or a character value, depending on the value of uCode
        ///     and uMapType.
        ///     If there is no translation, the return value is zero.
        /// </returns>
        public static int MapVirtualKey(Keys key, TranslationTypes translation)
        {
            return MapVirtualKey((int)key, translation);
        }

        /// <summary>
        ///     Places (posts) a message in the message queue associated with the thread that created the specified window and
        ///     returns without waiting for the thread to process the message.
        /// </summary>
        /// <param name="windowHandle">
        ///     A handle to the window whose window procedure is to receive the message. The following
        ///     values have special meanings.
        /// </param>
        /// <param name="message">The message to be posted.</param>
        /// <param name="wParam">Additional message-specific information.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        public static void PostMessage(IntPtr windowHandle, int message, IntPtr wParam, IntPtr lParam)
        {
            // Check if the handle is valid
            HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

            // Post the message
            if (!User32.PostMessage(windowHandle, message, wParam, lParam))
                throw new Win32Exception($"Couldn't post the message '{message}'.");
        }

        /// <summary>
        ///     Places (posts) a message in the message queue associated with the thread that created the specified window and
        ///     returns without waiting for the thread to process the message.
        /// </summary>
        /// <param name="windowHandle">
        ///     A handle to the window whose window procedure is to receive the message. The following
        ///     values have special meanings.
        /// </param>
        /// <param name="message">The message to be posted.</param>
        /// <param name="wParam">Additional message-specific information.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        public static void PostMessage(IntPtr windowHandle, WindowsMessages message, IntPtr wParam, IntPtr lParam)
        {
            PostMessage(windowHandle, (int) message, wParam, lParam);
        }

        /// <summary>
        ///     Synthesizes keystrokes, mouse motions, and button clicks.
        /// </summary>
        /// <param name="inputs">
        ///     An array of <see cref="Input" /> structures. Each structure represents an event to be inserted
        ///     into the keyboard or mouse input stream.
        /// </param>
        public static void SendInput(Input[] inputs)
        {
            // Check if the array passed in parameter is not empty
            if (inputs != null && inputs.Length != 0)
            {
                if (User32.SendInput(inputs.Length, inputs, MarshalType<Input>.Size) == 0)
                    throw new Win32Exception("Couldn't send the inputs.");
            }
            else
                throw new ArgumentException("The parameter cannot be null or empty.", "inputs");
        }

        /// <summary>
        ///     Synthesizes keystrokes, mouse motions, and button clicks.
        /// </summary>
        /// <param name="input">A structure represents an event to be inserted into the keyboard or mouse input stream.</param>
        public static void SendInput(Input input)
        {
            SendInput(new[] {input});
        }

        /// <summary>
        ///     Sends the specified message to a window or windows.
        ///     The SendMessage function calls the window procedure for the specified window and does not return until the window
        ///     procedure has processed the message.
        /// </summary>
        /// <param name="windowHandle">A handle to the window whose window procedure will receive the message.</param>
        /// <param name="message">The message to be sent.</param>
        /// <param name="wParam">Additional message-specific information.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
        public static IntPtr SendMessage(IntPtr windowHandle, int message, IntPtr wParam, IntPtr lParam)
        {
            // Check if the handle is valid
            HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

            // Send the message
            return User32.SendMessage(windowHandle, message, wParam, lParam);
        }

        /// <summary>
        ///     Sends the specified message to a window or windows.
        ///     The SendMessage function calls the window procedure for the specified window and does not return until the window
        ///     procedure has processed the message.
        /// </summary>
        /// <param name="windowHandle">A handle to the window whose window procedure will receive the message.</param>
        /// <param name="message">The message to be sent.</param>
        /// <param name="wParam">Additional message-specific information.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
        public static IntPtr SendMessage(IntPtr windowHandle, WindowsMessages message, IntPtr wParam, IntPtr lParam)
        {
            return SendMessage(windowHandle, (int) message, wParam, lParam);
        }

        /// <summary>
        ///     Sends the specified message to a window or windows.
        ///     The SendMessage function calls the window procedure for the specified window and does not return until the window
        ///     procedure has processed the message.
        /// </summary>
        /// <param name="message">The message data to send.</param>
        public static IntPtr SendMessage(Message message)
        {
            return SendMessage(message.HWnd, message.Msg, message.WParam, message.LParam);
        }

        /// <summary>
        ///     Brings the thread that created the specified window into the foreground and activates the window.
        ///     The window is restored if minimized. Performs no action if the window is already activated.
        /// </summary>
        /// <param name="windowHandle">A handle to the window that should be activated and brought to the foreground.</param>
        /// <returns>
        ///     If the window was brought to the foreground, the return value is <c>true</c>, otherwise the return value is
        ///     <c>false</c>.
        /// </returns>
        public static void SetForegroundWindow(IntPtr windowHandle)
        {
            // Check if the handle is valid
            HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

            // If the window is already activated, do nothing
            if (GetForegroundWindow() == windowHandle)
                return;

            // Restore the window if minimized
            ShowWindow(windowHandle, WindowStates.Restore);

            // Activate the window
            if (!User32.SetForegroundWindow(windowHandle))
                throw new ApplicationException("Couldn't set the window to foreground.");
        }

        /// <summary>
        ///     Sets the current position and size of the specified window.
        /// </summary>
        /// <param name="windowHandle">A handle to the window.</param>
        /// <param name="left">The x-coordinate of the upper-left corner of the window.</param>
        /// <param name="top">The y-coordinate of the upper-left corner of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <param name="width">The width of the window.</param>
        public static void SetWindowPlacement(IntPtr windowHandle, int left, int top, int height, int width)
        {
            // Check if the handle is valid
            HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

            // Get a WindowPlacement structure of the current window
            var placement = GetWindowPlacement(windowHandle);

            // Set the values
            placement.NormalPosition.Left = left;
            placement.NormalPosition.Top = top;
            placement.NormalPosition.Height = height;
            placement.NormalPosition.Width = width;

            // Set the window placement
            SetWindowPlacement(windowHandle, placement);
        }

        /// <summary>
        ///     Sets the show state and the restored, minimized, and maximized positions of the specified window.
        /// </summary>
        /// <param name="windowHandle">A handle to the window.</param>
        /// <param name="placement">
        ///     A pointer to the <see cref="WindowPlacement" /> structure that specifies the new show state and
        ///     window positions.
        /// </param>
        public static void SetWindowPlacement(IntPtr windowHandle, WindowPlacement placement)
        {
            // Check if the handle is valid
            HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

            // If the debugger is attached and the state of the window is ShowDefault, there's an issue where the window disappears
            if (Debugger.IsAttached && placement.ShowCmd == WindowStates.ShowNormal)
                placement.ShowCmd = WindowStates.Restore;

            // Set the window placement
            if (!User32.SetWindowPlacement(windowHandle, ref placement))
                throw new Win32Exception("Couldn't set the window placement.");
        }

        /// <summary>
        ///     Sets the text of the specified window's title bar.
        /// </summary>
        /// <param name="windowHandle">A handle to the window whose text is to be changed.</param>
        /// <param name="title">The new title text.</param>
        public static void SetWindowText(IntPtr windowHandle, string title)
        {
            // Check if the handle is valid
            HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

            // Set the text of the window's title bar
            if (!User32.SetWindowText(windowHandle, title))
                throw new Win32Exception("Couldn't set the text of the window's title bar.");
        }

        /// <summary>
        ///     Sets the specified window's show state.
        /// </summary>
        /// <param name="windowHandle">A handle to the window.</param>
        /// <param name="state">Controls how the window is to be shown.</param>
        /// <returns>
        ///     If the window was previously visible, the return value is <c>true</c>, otherwise the return value is
        ///     <c>false</c>.
        /// </returns>
        public static bool ShowWindow(IntPtr windowHandle, WindowStates state)
        {
            // Check if the handle is valid
            HandleManipulator.ValidateAsArgument(windowHandle, "windowHandle");

            // Change the state of the window
            return User32.ShowWindow(windowHandle, state);
        }

        /// <summary>
        /// Gets the main handle
        /// </summary>
        /// <param name="processName">Name of the process.</param>
        /// <returns>IntPtr.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IntPtr GetMainWindowHandle(string processName)
        {
            var firstOrDefault = System.Diagnostics.Process.GetProcessesByName(processName);

            var process = firstOrDefault.FirstOrDefault();

            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            return process.MainWindowHandle;
        }

    }
}