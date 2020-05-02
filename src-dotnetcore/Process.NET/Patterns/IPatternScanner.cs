namespace Process.NET.Patterns
{
    public interface IPatternScanner
    {
        PatternScanResult Find(IMemoryPattern pattern);
    }
}