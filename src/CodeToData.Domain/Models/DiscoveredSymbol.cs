using System;
using Microsoft.CodeAnalysis;

namespace CodeToData.Domain.Models
{
    public class DiscoveredSymbol : IEquatable<DiscoveredSymbol>
    {
        public DiscoveredSymbol(INamedTypeSymbol namedTypeSymbol)
        {
            _symbol = namedTypeSymbol;
        }

        public string SymbolName => _symbol.Name;
        public string ContainingAssembly => _symbol.ContainingAssembly.Name;
        public string ContainingNamespace => _symbol.ContainingNamespace.ToDisplayString();
        public string TypeKind => _symbol.TypeKind.ToString();
        private INamedTypeSymbol _symbol { get; }


        public bool Equals(DiscoveredSymbol other)
        {
            if (ReferenceEquals(null, other)) return false;

            if (ReferenceEquals(this, other)) return true;

            return string.Equals(ContainingAssembly, other.ContainingAssembly) &&
                   string.Equals(ContainingNamespace, other.ContainingNamespace) &&
                   string.Equals(SymbolName, other.SymbolName) &&
                   string.Equals(TypeKind, other.TypeKind);
        }

        public INamedTypeSymbol Symbol()
        {
            return _symbol;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (obj.GetType() != GetType()) return false;

            return string.Equals(ContainingAssembly, ((DiscoveredSymbol)obj).ContainingAssembly) &&
                   string.Equals(ContainingNamespace, ((DiscoveredSymbol)obj).ContainingNamespace) &&
                   string.Equals(SymbolName, ((DiscoveredSymbol)obj).SymbolName) &&
                   string.Equals(TypeKind, ((DiscoveredSymbol)obj).TypeKind);
        }

        public override int GetHashCode()
        {
            var hashCode = ContainingAssembly != null ? ContainingAssembly.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ (ContainingNamespace != null ? ContainingNamespace.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (SymbolName != null ? SymbolName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (TypeKind != null ? TypeKind.GetHashCode() : 0);

            return hashCode;
        }
    }
}