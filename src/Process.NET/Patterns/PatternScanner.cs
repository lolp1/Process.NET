using System;
using System.Linq;
using Process.NET.Modules;
using Process.NET.Utilities;

namespace Process.NET.Patterns
{
    public class PatternScanner : IPatternScanner
    {
        private readonly IProcessModule _module;
        private readonly int _offsetFromBaseAddress;

        private static readonly PatternScanResult EmptyPatternScanResult = new PatternScanResult
        {
            BaseAddress = IntPtr.Zero,
            ReadAddress = IntPtr.Zero,
            Offset = 0,
            Found = false
        };

        public PatternScanner(IProcessModule module) : this(module, 0) { }
        public PatternScanner(IProcessModule module, int offsetFromBaseAddress)
        {
            _module = module;
            _offsetFromBaseAddress = offsetFromBaseAddress;
            Data = module.Read(_offsetFromBaseAddress, _module.Size - _offsetFromBaseAddress);
        }

        public byte[] Data { get; }

        public PatternScanResult Find(IMemoryPattern pattern)
        {
            switch(pattern.PatternType)
            {
                case MemoryPatternType.Function:
                    return FindFunctionPattern(pattern);
                case MemoryPatternType.Data:
                    return FindDataPattern(pattern);
            }
            throw new NotImplementedException("PatternScanner encountered an unknown MemoryPatternType: " + pattern.PatternType + ".");
        }

        

        private int GetOffset(IMemoryPattern pattern)
        {
            switch(pattern.Algorithm)
            {
                case PatternScannerAlgorithm.BoyerMooreHorspool:
                    return Utilities.BoyerMooreHorspool.IndexOf(Data, pattern.GetBytes().ToArray());
                case PatternScannerAlgorithm.Naive:
                    return Utilities.Naive.GetIndexOf(pattern, Data, _module);
            }
            throw new NotImplementedException("GetOffset encountered an unknown PatternScannerAlgorithm: " + pattern.Algorithm + ".");
        }

        private PatternScanResult FindFunctionPattern(IMemoryPattern pattern)
        {
            var offset = GetOffset(pattern);
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
            return EmptyPatternScanResult;
        }

        private PatternScanResult FindDataPattern(IMemoryPattern pattern)
        {
            var result = new PatternScanResult();
            var offset = GetOffset(pattern);

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
            return EmptyPatternScanResult;
        }
    }
}