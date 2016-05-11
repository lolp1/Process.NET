namespace Process.NET.Windows.Mouse
{
    public interface IMouse
    {
        void ClickLeft();
        void ClickMiddle();
        void ClickRight();
        void DoubleClickLeft();
        void MoveTo(int x, int y);
        void PressLeft();
        void PressMiddle();
        void PressRight();
        void ReleaseLeft();
        void ReleaseMiddle();
        void ReleaseRight();
        void ScrollHorizontally(int delta = 120);
        void ScrollVertically(int delta = 120);
    }
}