using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OzCompress
{
    public static class BitArrayExtensions
    {
        public static string ToBitString(this BitArray bits)
        {
            return ToBitString(bits, 0, bits.Length);

            //var sb = new StringBuilder();

            //for (int i = 0; i < bits.Count; i++)
            //{
            //    char c = bits[i] ? '1' : '0';
            //    sb.Append(c);
            //}

            //return sb.ToString();
        }

        public static string ToBitString(this BitArray bits, int start, int length)
        {
            var sb = new StringBuilder();

            for (var i = start; i < start + length; i++)
            {
                char c = bits[i] ? '1' : '0';
                sb.Append(c);
            }

            return sb.ToString();
        }

        public static BitArray ToBitArray(this string str)
        {
            return str.ToBitArray(0, str.Length);
            //var bits = new BitArray(str.Length);

            //for (int i = 0; i < str.Length; i++)
            //{
            //    bits[i] = str[i] == '1';
            //}

            //return bits;
        }

        public static BitArray ToBitArray(this string str, int start, int length)
        {
            var bits = new BitArray(length);

            for (int i = start; i < start + length; i++)
            {
                bits[i] = str[i] == '1';
            }

            return bits;
        }

        public static string ToBitString(this BitArray bits, Codon codon)
        {
            return bits.ToBitString(codon.Index, codon.Length);
        }

        public static BitArray TakeLeft(this BitArray bits, int length)
        {
            if(length == 0)
                return new BitArray(bits);

            var newBits = new BitArray(bits.Length - length);
            for (int i = 0; i < bits.Length - length; i++)
            {
                newBits[i] = bits[i+length];
            }

            return newBits;
        }

        public static BitArray Sub(this BitArray bits, int start, int length)
        {
            if (length == 0)
                return new BitArray(0);

            var newBits = new List<bool>(length);
            for (int i = 0; i < length; i++)
            {
                if (start + i >= bits.Length)
                    break;

                newBits.Add(bits[start + i]);
            }

            return new BitArray(newBits.ToArray());
        }
    }
}