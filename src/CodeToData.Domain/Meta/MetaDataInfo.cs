using Microsoft.CodeAnalysis;

namespace CodeToData.Domain.Meta
{
    public class MetaDataInfo
    {
        public MetaDataInfo(ISymbol found, ISymbol foundIn)
        {
            Found = found;
            FoundIn = foundIn;
        }

        public ISymbol Found { get; set; }
        public ISymbol FoundIn { get; set; }
    }
}