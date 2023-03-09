namespace CodeToData.Domain.Models
{
    public class DiscoveredType
    {
        public string Name { get; set; }
        public string SourceAssembly { get; set; }
        public string SourceNamespace { get; set; }
        public string TypeKind { get; set; }
        public string AssemblyName { get; set; }
        public string DocumentName { get; set; }
        public int StartPosition { get; set; }
        public int EndPosition { get; set; }
    }
}