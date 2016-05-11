namespace Process.NET.Windows.Mouse
{
    public interface IMouse
    {
        /// <summary>
        ///     Clicks the left button of the mouse at the current cursor position.
        /// </summary>
        void ClickLeft();

        /// <summary>
        ///     Clicks the middle button of the mouse at the current cursor position.
        /// </summary>
        void ClickMiddle();

        /// <summary>
        ///     Clicks the right button of the mouse at the current cursor position.
        /// </summary>
        void ClickRight();

        /// <summary>
        ///     Double clicks the left button of the mouse at the current cursor position.
        /// </summary>
        void DoubleClickLeft();

        /// <summary>
        ///     Moves the cursor at the specified coordinate from the position of the window.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        void MoveTo(int x, int y);

        /// <summary>
        ///     Presses the left button of the mouse at the current cursor position.
        /// </summary>
        void PressLeft();

        /// <summary>
        ///     Presses the middle button of the mouse at the current cursor position.
        /// </summary>
        void PressMiddle();

        /// <summary>
        ///     Presses the right button of the mouse at the current cursor position.
        /// </summary>
        void PressRight();

        /// <summary>
        ///     Releases the left button of the mouse at the current cursor position.
        /// </summary>
        void ReleaseLeft();

        /// <summary>
        ///     Releases the middle button of the mouse at the current cursor position.
        /// </summary>
        void ReleaseMiddle();

        /// <summary>
        ///     Releases the right button of the mouse at the current cursor position.
        /// </summary>
        void ReleaseRight();

        /// <summary>
        ///     Scrolls horizontally using the wheel of the mouse at the current cursor position.
        /// </summary>
        /// <param name="delta">The amount of wheel movement.</param>
        void ScrollHorizontally(int delta = 120);

        /// <summary>
        ///     Scrolls vertically using the wheel of the mouse at the current cursor position.
        /// </summary>
        /// <param name="delta">The amount of wheel movement.</param>
        void ScrollVertically(int delta = 120);
    }
}