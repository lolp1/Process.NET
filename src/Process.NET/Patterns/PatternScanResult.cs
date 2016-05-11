using System;

namespace Process.NET.Patterns
{
    public struct PatternScanResult
    {
        public IntPtr ReadAddress { get; set; }
        public IntPtr BaseAddress { get; set; }
        public int Offset { get; set; }
        public bool Found { get; set; }
    }
}