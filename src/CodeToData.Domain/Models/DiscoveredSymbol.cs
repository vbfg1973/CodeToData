using System;
using Microsoft.CodeAnalysis;

namespace CodeToData.Domain.Models;

public class DiscoveredSymbol : IEquatable<DiscoveredSymbol>
{
    public DiscoveredSymbol(INamedTypeSymbol namedTypeSymbol)
    {
        _symbol = namedTypeSymbol;
    }

    public string Assembly => _symbol.ContainingAssembly.Name;
    public string Namespace => _symbol.ContainingNamespace.ToDisplayString();
    public string Name => _symbol.Name;
    private INamedTypeSymbol _symbol { get; }

    public bool Equals(DiscoveredSymbol other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(Assembly, other.Assembly) &&
               string.Equals(Namespace, other.Namespace) &&
               string.Equals(Name, other.Name);
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
        return string.Equals(Assembly, ((DiscoveredSymbol)obj).Assembly) &&
               string.Equals(Namespace, ((DiscoveredSymbol)obj).Namespace) &&
               string.Equals(Name, ((DiscoveredSymbol)obj).Name);
    }

    public override int GetHashCode()
    {
        var hashCode = Assembly != null ? Assembly.GetHashCode() : 0;
        hashCode = (hashCode * 397) ^ (Namespace != null ? Namespace.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);

        return hashCode;
    }
}