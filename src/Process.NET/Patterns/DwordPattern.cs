using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Process.NET.Patterns
{
    public class DwordPattern : IMemoryPattern
    {
        private readonly byte[] _bytes;
        private readonly string _mask;

        public readonly string PatternText;

        public DwordPattern(string dwordPattern)
        {
            PatternText = dwordPattern;
            PatternType = MemoryPatternType.Function;
            Offset = 0;
            _bytes = GetBytesFromDwordPattern(dwordPattern);
            _mask = GetMaskFromDwordPattern(dwordPattern);
        }

        public DwordPattern(string pattern, int offset)
        {
            PatternText = pattern;
            PatternType = MemoryPatternType.Data;
            Offset = offset;
            _bytes = GetBytesFromDwordPattern(pattern);
            _mask = GetMaskFromDwordPattern(pattern);
        }

        public IList<byte> GetBytes()
        {
            return _bytes;
        }

        public string GetMask()
        {
            return _mask;
        }

        public int Offset { get; }
        public MemoryPatternType PatternType { get; }

        private static string GetMaskFromDwordPattern(string pattern)
        {
            var mask = pattern.Split(' ').Select(s => s.Contains('?') ? "?" : "x");

            return string.Concat(mask);
        }

        private static byte[] GetBytesFromDwordPattern(string pattern)
        {
            return
                pattern.Split(' ')
                    .Select(s => s.Contains('?') ? (byte) 0 : byte.Parse(s, NumberStyles.HexNumber))
                    .ToArray();
        }

        public override string ToString()
        {
            return PatternText;
        }
    }
}