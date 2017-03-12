using System.Collections;

namespace OzCompress
{
    public class Searcher
    {
        public static Codon Search(BitArray list, BitArray toSearch, 
            int startIndex,
            int maxLength)
        {
            var lastCodon = new Codon();
            
            if (toSearch.Length < maxLength)
                maxLength = toSearch.Length;

            for (int i = startIndex; i < list.Length; i++)
            {
                var length = 0;
                for (var j = 0; j < toSearch.Length; j++)
                {
                    if (i + j > list.Length-1) // end of list
                    {
                        if (length > lastCodon.Length)
                            return new Codon(i, length);
                        
                        return lastCodon;
                    }

                    if (list[i + j] != toSearch[j])
                    {
                        // found a bigger codon
                        if (length > lastCodon.Length)
                            lastCodon = new Codon(i, length);
                        break;
                    }
                    
                    length++;

                    // reached the max length
                    if (length == maxLength)
                        return new Codon(i, length);
                }
            }
            
            return lastCodon;
        }

        public static int SearchShift(BitArray list, BitArray toSearch,
            int startIndex,
            int maxLength)
        {
            int shift = -1;
            Codon codon;
            do
            {
                shift++;
                var searchBits = toSearch.TakeLeft(shift);

                if (searchBits.Length == 0)
                    break;

                codon = Searcher.Search(list, searchBits, 
                    startIndex, maxLength);

            } while (codon.Length < maxLength && shift < maxLength);

            return shift;
        }
    }
}
