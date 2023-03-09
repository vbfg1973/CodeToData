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
}