namespace CodeToData.Domain.Models
{
    public class TypeDefinition
    {
        public TypeDefinition(string filename, string ns, string type)
        {
            Filename = filename;
            Namespace = ns;
            Type = type;
        }

        public string Filename { get; }
        public string Namespace { get; }
        public string Type { get; }
    }
    
    public record CodeSignature
    {
        public int Signature { get; set; }
        public string Project { get; set; }
        public string Namespace { get; set; }
        public string Filename { get; set; }
        public int Descendants { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public string ChildNodeString { get; set; }
        public string CodeSnippet { get; set; }
    }
}