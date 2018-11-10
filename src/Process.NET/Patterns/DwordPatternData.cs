using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Process.NET.Patterns
{
    public class DwordPatternData : IMemoryPattern
    {
        private readonly byte[] _bytes;
        private readonly string _mask;

        public readonly string PatternText;

        public DwordPatternData(string pattern) : this(pattern, 0, PatternScannerAlgorithm.Naive) { }
        public DwordPatternData(string pattern, int offset) : this(pattern, offset, PatternScannerAlgorithm.Naive) { }
        public DwordPatternData(string pattern, PatternScannerAlgorithm algorithm) : this(pattern, 0, algorithm) { }
        public DwordPatternData(string pattern, int offset, PatternScannerAlgorithm algorithm)
        {
            PatternText = pattern;
            PatternType = MemoryPatternType.Data;
            Offset = offset;
            Algorithm = algorithm;
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
        public PatternScannerAlgorithm Algorithm { get; }

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