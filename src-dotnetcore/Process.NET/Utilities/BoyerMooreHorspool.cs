namespace Process.NET.Utilities
{
    //Original C++ implementation by DarthTon. Ported to C# by gir489.
    public class BoyerMooreHorspool
    {
        private const byte WildCard = 0x00;
        
        private static int[] BuildBadCharTable(byte[] pPattern)
        {
            int idx = 0;
            int last = pPattern.Length - 1;
            int[] badShift = new int[256];

            // Get last wildcard position
            for (idx = last; idx > 0 && pPattern[idx] != WildCard; --idx) ;
            int diff = last - idx;
            if (diff == 0)
                diff = 1;

            // Prepare shift table
            for (idx = 0; idx <= 255; ++idx)
                badShift[idx] = diff;
            for (idx = last - diff; idx < last; ++idx)
                badShift[pPattern[idx]] = last - idx;
            return badShift;
        }

        public static int IndexOf(byte[] buffer, byte[] pattern)
        {
            if (pattern.Length > buffer.Length)
            {
                return -1;
            }
            int[] badShift = BuildBadCharTable(pattern);
            int offset = 0;
            int position = 0;
            int last = pattern.Length - 1;
            int maxoffset = buffer.Length - pattern.Length;
            while (offset <= maxoffset)
            {
                for (position = last; (pattern[position] == buffer[position + offset] || pattern[position] == WildCard); position--)
                {
                    if (position == 0)
                    {
                        return offset;
                    }
                }
                offset += badShift[(int)buffer[offset + last]];
            }
            return -1;
        }
    }
}
