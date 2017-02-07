using System;
using System.Linq;
using Process.NET.Modules;

namespace Process.NET.Patterns
{
    public class PatternScanner : IPatternScanner
    {
        private readonly IProcessModule _module;
        private readonly int _offsetFromBaseAddress;

        public PatternScanner(IProcessModule module)
        {
            new PatternScanner(module, 0);
        }

        public PatternScanner(IProcessModule module, int offsetFromBaseAddress)
        {
            _module = module;
            _offsetFromBaseAddress = offsetFromBaseAddress;
            Data = module.Read(_offsetFromBaseAddress, _module.Size - _offsetFromBaseAddress);
        }

        public byte[] Data { get; }

        public PatternScanResult Find(IMemoryPattern pattern)
        {
            return pattern.PatternType == MemoryPatternType.Function
                ? FindFunctionPattern(pattern)
                : FindDataPattern(pattern);
        }

        private PatternScanResult FindFunctionPattern(IMemoryPattern pattern)
        {
            var patternData = Data;
            var patternDataLength = patternData.Length;

            var offset = Utilities.BoyerMooreHorspool.IndexOf(Data, pattern.GetBytes().ToArray());

            if (offset != -1)
            {
                return new PatternScanResult
                {
                    BaseAddress = _module.BaseAddress + offset + _offsetFromBaseAddress,
                    ReadAddress = _module.BaseAddress + offset + _offsetFromBaseAddress,
                    Offset = offset + _offsetFromBaseAddress,
                    Found = true
                };
            }
            return new PatternScanResult
            {
                BaseAddress = IntPtr.Zero,
                ReadAddress = IntPtr.Zero,
                Offset = 0,
                Found = false
            };
        }

        private PatternScanResult FindDataPattern(IMemoryPattern pattern)
        {
            var patternData = Data;
            var patternBytes = pattern.GetBytes();
            var patternMask = pattern.GetMask();

            var result = new PatternScanResult();
            var offset = Utilities.BoyerMooreHorspool.IndexOf(Data, pattern.GetBytes().ToArray());

            if ( offset != -1)
            {
                // If this area is reached, the pattern has been found.
                result.Found = true;
                result.ReadAddress = _module.Read<IntPtr>(offset + pattern.Offset);
                result.BaseAddress = new IntPtr(result.ReadAddress.ToInt64() - _module.BaseAddress.ToInt64());
                result.Offset = offset;
                return result;
            }
            // If this is reached, the pattern was not found.
            result.Found = false;
            result.Offset = 0;
            result.ReadAddress = IntPtr.Zero;
            result.BaseAddress = IntPtr.Zero;
            return result;
        }
    }
}