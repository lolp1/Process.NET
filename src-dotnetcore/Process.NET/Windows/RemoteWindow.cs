using System;
using System.Collections.Generic;
using System.Linq;
using Process.NET.Native.Types;
using Process.NET.Threads;
using Process.NET.Utilities;
using Process.NET.Windows.Keyboard;
using Process.NET.Windows.Mouse;

namespace Process.NET.Windows
{
    /// <summary>
    ///     Class repesenting a window in the remote process.
    /// </summary>
    public class RemoteWindow : IEquatable<RemoteWindow>, IWindow
    {
        protected readonly IProcess ProcessPlus;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RemoteWindow" /> class.
        /// </summary>
        /// <param name="processPlus">The reference of the <see cref="IProcess" /> object.</param>
        /// <param name="handle">The handle of a window.</param>
        public RemoteWindow(IProcess processPlus, IntPtr handle)
        {
            // Save the parameters
            ProcessPlus = processPlus;
            Handle = handle;
            // Create the tools
            Keyboard = new MessageKeyboard(this);
            Mouse = new SendInputMouse(this);
        }

        /// <summary>
        ///     Gets all the child window handles of this window.
        /// </summary>
        protected IEnumerable<IntPtr> ChildrenHandles => WindowHelper.EnumChildWindows(Handle);

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        public bool Equals(RemoteWindow other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(ProcessPlus, other.ProcessPlus) && Handle.Equals(other.Handle);
        }

        /// <summary>
        ///     Gets all the child windows of this window.
        /// </summary>
        public IEnumerable<IWindow> Children
        {
            get { return ChildrenHandles.Select(handle => new RemoteWindow(ProcessPlus, handle)); }
        }

        /// <summary>
        ///     Gets the class name of the window.
        /// </summary>
        public string ClassName => WindowHelper.GetClassName(Handle);

        /// <summary>
        ///     The handle of the window.
        /// </summary>
        /// <remarks>
        ///     The type here is not <see cref="SafeMemoryHandle" /> because a window cannot be closed by calling
        ///     <see cref="NativeMethods.CloseHandle" />.
        ///     For more information, see:
        ///     http://stackoverflow.com/questions/8507307/why-cant-i-close-the-window-handle-in-my-code.
        /// </remarks>
        public IntPtr Handle { get; }

        /// <summary>
        ///     Gets or sets the height of the element.
        /// </summary>
        public int Height
        {
            get { return Placement.NormalPosition.Height; }
            set
            {
                var p = Placement;
                p.NormalPosition.Height = value;
                Placement = p;
            }
        }

        /// <summary>
        ///     Gets if the window is currently activated.
        /// </summary>
        public bool IsActivated => WindowHelper.GetForegroundWindow() == Handle;

        /// <summary>
        ///     Gets if this is the main window.
        /// </summary>
        public bool IsMainWindow => ProcessPlus.Native.MainWindowHandle == Handle;

        /// <summary>
        ///     Tools for managing a virtual keyboard in the window.
        /// </summary>
        public IKeyboard Keyboard { get; set; }

        /// <summary>
        ///     Tools for managing a virtual mouse in the window.
        /// </summary>
        public IMouse Mouse { get; set; }

        /// <summary>
        ///     Gets or sets the placement of the window.
        /// </summary>
        public WindowPlacement Placement
        {
            get { return WindowHelper.GetWindowPlacement(Handle); }
            set { WindowHelper.SetWindowPlacement(Handle, value); }
        }

        /// <summary>
        ///     Gets or sets the specified window's show state.
        /// </summary>
        public WindowStates State
        {
            get { return Placement.ShowCmd; }
            set { WindowHelper.ShowWindow(Handle, value); }
        }

        /// <summary>
        ///     Gets or sets the title of the window.
        /// </summary>
        public string Title
        {
            get { return WindowHelper.GetWindowText(Handle); }
            set { WindowHelper.SetWindowText(Handle, value); }
        }

        /// <summary>
        ///     Gets the thread of the window.
        /// </summary>
        public IRemoteThread Thread => ProcessPlus.ThreadFactory.GetThreadById(WindowHelper.GetWindowThreadId(Handle));

        /// <summary>
        ///     Gets or sets the width of the element.
        /// </summary>
        public int Width
        {
            get { return Placement.NormalPosition.Width; }
            set
            {
                var p = Placement;
                p.NormalPosition.Width = value;
                Placement = p;
            }
        }

        /// <summary>
        ///     Gets or sets the x-coordinate of the window.
        /// </summary>
        public int X
        {
            get { return Placement.NormalPosition.Left; }
            set
            {
                var p = Placement;
                p.NormalPosition.Right = value + p.NormalPosition.Width;
                p.NormalPosition.Left = value;
                Placement = p;
            }
        }

        /// <summary>
        ///     Gets or sets the y-coordinate of the window.
        /// </summary>
        public int Y
        {
            get { return Placement.NormalPosition.Top; }
            set
            {
                var p = Placement;
                p.NormalPosition.Bottom = value + p.NormalPosition.Height;
                p.NormalPosition.Top = value;
                Placement = p;
            }
        }

        /// <summary>
        ///     Activates the window.
        /// </summary>
        public void Activate()
        {
            WindowHelper.SetForegroundWindow(Handle);
        }

        /// <summary>
        ///     Closes the window.
        /// </summary>
        public void Close()
        {
            PostMessage(WindowsMessages.Close, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        ///     Flashes the window one time. It does not change the active state of the window.
        /// </summary>
        public void Flash()
        {
            WindowHelper.FlashWindow(Handle);
        }

        /// <summary>
        ///     Flashes the window. It does not change the active state of the window.
        /// </summary>
        /// <param name="count">The number of times to flash the window.</param>
        /// <param name="timeout">The rate at which the window is to be flashed.</param>
        /// <param name="flags">The flash status.</param>
        public void Flash(int count, TimeSpan timeout, FlashWindowFlags flags = FlashWindowFlags.All)
        {
            WindowHelper.FlashWindowEx(Handle, flags, count, timeout);
        }

        public virtual void Dispose()
        {
        }

        /// <summary>
        ///     Places (posts) a message in the message queue associated with the thread that created the window and returns
        ///     without waiting for the thread to process the message.
        /// </summary>
        /// <param name="message">The message to be posted.</param>
        /// <param name="wParam">Additional message-specific information.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        public void PostMessage(WindowsMessages message, IntPtr wParam, IntPtr lParam)
        {
            WindowHelper.PostMessage(Handle, message, wParam, lParam);
        }

        /// <summary>
        ///     Places (posts) a message in the message queue associated with the thread that created the window and returns
        ///     without waiting for the thread to process the message.
        /// </summary>
        /// <param name="message">The message to be posted.</param>
        /// <param name="wParam">Additional message-specific information.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        public void PostMessage(int message, IntPtr wParam, IntPtr lParam)
        {
            WindowHelper.PostMessage(Handle, message, wParam, lParam);
        }

        /// <summary>
        ///     Sends the specified message to a window or windows.
        ///     The SendMessage function calls the window procedure for the specified window and does not return until the window
        ///     procedure has processed the message.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="wParam">Additional message-specific information.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
        public IntPtr SendMessage(WindowsMessages message, IntPtr wParam, IntPtr lParam)
        {
            return WindowHelper.SendMessage(Handle, message, wParam, lParam);
        }

        /// <summary>
        ///     Sends the specified message to a window or windows.
        ///     The SendMessage function calls the window procedure for the specified window and does not return until the window
        ///     procedure has processed the message.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="wParam">Additional message-specific information.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
        public IntPtr SendMessage(int message, IntPtr wParam, IntPtr lParam)
        {
            return WindowHelper.SendMessage(Handle, message, wParam, lParam);
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ProcessPlus?.GetHashCode() ?? 0;
                hashCode = (hashCode*397) ^ Handle.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        ///     Determines whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RemoteWindow) obj);
        }

        public static bool operator ==(RemoteWindow left, RemoteWindow right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RemoteWindow left, RemoteWindow right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return $"Title = {Title} ClassName = {ClassName}";
        }
    }
}