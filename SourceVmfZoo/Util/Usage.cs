namespace SourceVmfZoo.Util
{
    public class Usage
    {
        public string Name { get; }
        public int Count { get; }

        public Usage(string name, int count)
        {
            Name = name;
            Count = count;
        }

        public override string ToString()
        {
            return $"{Count} - {Name}";
        }
    }
}