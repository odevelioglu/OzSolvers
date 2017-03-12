namespace OzCompress
{
    public class Codon
    {
        public int Index { get; set; }
        public int Length { get; set; }

        public Codon(): this(0,0) { }
        public Codon(int index, int length)
        {
            Index = index;
            Length = length;
        }

        public bool IsEmpty()
        {
            return this.Length == 0;
        }

        public override string ToString()
        {
            return string.Format("Index: {0}, Length: {1}", Index, Length);
        }
    }
}