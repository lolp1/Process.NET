using Process.NET.Modules;
using Process.NET.Patterns;
using System.Linq;

namespace Process.NET.Utilities
{
    public class Naive
    {
        public static int GetIndexOf(IMemoryPattern pattern, byte[] Data, IProcessModule module)
        {
            var patternData = Data;
            var patternDataLength = patternData.Length;

            for (var offset = 0; offset < patternDataLength; offset++)
            {
                if (
                    pattern.GetMask()
                        .Where((m, b) => m == 'x' && pattern.GetBytes()[b] != patternData[b + offset])
                        .Any())
                    continue;

                return offset;
            }
            return -1;
        }
    }
}
