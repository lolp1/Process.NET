using System.Collections.Generic;

namespace Process.NET.Patterns
{
    public interface IMemoryPattern
    {
        int Offset { get; }
        MemoryPatternType PatternType { get; }
        IList<byte> GetBytes();
        string GetMask();
    }
}