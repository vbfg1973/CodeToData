using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace CodeToData.Domain.Visitors.Semantics
{
    /// <summary>
    ///     Compiler is dumb af. SyntaxWalker and lookup of discovered nodes against semantic model is much more comprehensive
    /// </summary>
    public class NamedTypeDiscoveryVisitor : SymbolVisitor
    {
        private readonly ConcurrentDictionary<string, INamedTypeSymbol> _symbols;

        public NamedTypeDiscoveryVisitor()
        {
            _symbols = new ConcurrentDictionary<string, INamedTypeSymbol>();
        }

        public IEnumerable<INamedTypeSymbol> Symbols => _symbols
            .Values
            .OrderBy(symbol => symbol.ContainingAssembly.Name)
            .ThenBy(symbol => symbol.Name)
            .ToList();

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            foreach (var childSymbol in symbol.GetMembers())
            {
                childSymbol.Accept(this);
            }

            base.VisitNamespace(symbol);
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            var type = $"{symbol.ContainingAssembly.Name}_{symbol.Name}";

            _symbols[type] = symbol;

            foreach (var childSymbol in symbol.GetTypeMembers())
            {
                childSymbol.Accept(this);
            }

            Parallel.ForEach(symbol.GetTypeMembers(), s => s.Accept(this));

            Parallel.ForEach(symbol.GetMembers()
                    .Concat(symbol.Constructors.Where(x => !x.IsImplicitlyDeclared)),
                s => s.Accept(this));
        }
    }
}