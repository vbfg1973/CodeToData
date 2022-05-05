using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace CodeToData.Domain.Visitors.Semantics;

public class NamedTypeDiscoveryVisitor : SymbolVisitor
{
    private readonly Dictionary<string, INamedTypeSymbol> _symbols;

    public NamedTypeDiscoveryVisitor()
    {
        _symbols = new Dictionary<string, INamedTypeSymbol>();
    }

    public IEnumerable<INamedTypeSymbol> Symbols => _symbols
        .Values
        .OrderBy(symbol => symbol.ContainingAssembly.Name)
        .ThenBy(symbol => symbol.Name)
        .ToList();

    public override void VisitNamespace(INamespaceSymbol symbol)
    {
        foreach (var childSymbol in symbol.GetMembers()) childSymbol.Accept(this);
    }

    public override void VisitNamedType(INamedTypeSymbol symbol)
    {
        var type = $"{symbol.ContainingAssembly.Name}_{symbol.Name}";

        _symbols[type] = symbol;

        foreach (var childSymbol in symbol.GetTypeMembers()) childSymbol.Accept(this);
    }
}