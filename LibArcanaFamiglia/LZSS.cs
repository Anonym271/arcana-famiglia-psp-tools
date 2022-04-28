using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibArcanaFamiglia
{
    internal static class LZSS
    {
        private const int 
            EI = 12,
            EJ = 4,
            P = 2,
            rless = P;

        public static byte[] Decompress(byte[] input, int decompressedSize)
        {
            uint N = 1 << EI;
            uint F = 1 << EJ;

            byte[] output = new byte[decompressedSize];
            byte[] slideWin = new byte[N];

            int dst = 0;
            int src = 0;
            uint r = (N - F) - rless;
            N--;
            F--;

            byte c;
            uint i, j;

            for (uint flags = 0; ; flags >>= 1)
            {
                if ((flags & 0x100) == 0)
                {
                    if (src >= input.Length)
                        break;
                    flags = input[src++];
                    flags |= 0xff00;
                }
                if ((flags & 1) != 0)
                {
                    if (src >= input.Length)
                        break;
                    c = input[src++];
                    if (dst >= output.Length)
                        return output;
                    output[dst++] = c;
                    slideWin[r] = c;
                    r = (r + 1) & N;
                }
                else
                {
                    if (src >= input.Length)
                        break;
                    i = input[src++];
                    if (src >= input.Length)
                        break;
                    j = input[src++];
                    i |= ((j >> EJ) << 8);
                    j = (j & F) + P;
                    for (uint k = 0; k <= j; k++)
                    {
                        c = slideWin[(i + k) & N];
                        if (dst >= output.Length)
                            return output;
                        output[dst++] = c;
                        slideWin[r] = c;
                        r = (r + 1) & N;
                    }
                }
            }

            return output;
        }
    }
}
