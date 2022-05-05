using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeToData.Domain.Models;
using CodeToData.Domain.Services.CodeAnalysis;
using CodeToData.Domain.Visitors.Semantics;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace CodeToData.Domain.Verbs.SymbolFinder;

public class FinderVerb
{
    private readonly ILoggerFactory _loggerFactory;
    private ILogger<FinderVerb> _logger;

    public FinderVerb(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<FinderVerb>();
    }

    public async Task Run(FinderOptions options)
    {
        await Console.Error.WriteLineAsync(
            $"Compiling {options.Solution}");
        var compiler = new SolutionCompilerService(options.Solution);

        var compilations = new Dictionary<string, Compilation>();
        var symbols = new HashSet<DiscoveredSymbol>();

        foreach (var compilation in compiler.Compilations)
        {
            await Console.Error.WriteLineAsync($"Collating symbols available to {compilation.AssemblyName}");
            compilations[compilation.AssemblyName] = compilation;
            var symbolDiscovery = new NamedTypeDiscoveryVisitor();
            symbolDiscovery.Visit(compilation.GlobalNamespace);

            foreach (var symbol in symbolDiscovery.Symbols) symbols.Add(new DiscoveredSymbol(symbol));
        }

        if (options.NamespaceFilter != null)
            symbols = symbols
                .Where(x => x.Namespace.Contains(options.NamespaceFilter, StringComparison.InvariantCultureIgnoreCase))
                .ToHashSet();

        var output = new List<dynamic>();
        foreach (var symbol in symbols)
        {
            var references =
                (await Microsoft.CodeAnalysis.FindSymbols.SymbolFinder.FindReferencesAsync(symbol.Symbol(),
                    compiler.Solution)).ToList();

            output.AddRange(from reference in references
                from location in reference.Locations
                select new
                {
                    SymbolName = symbol.Name,
                    ContainingAssembly = symbol.Symbol().ContainingAssembly.Name,
                    ContainingNamespace = symbol.Symbol().ContainingNamespace.ToDisplayString(),
                    Project = location.Document.Project.Name,
                    Document = location.Document.FilePath,
                    StartPosition = location.Location.SourceSpan.Start,
                    EndPosition = location.Location.SourceSpan.End
                });
        }

        await Utilities.SaveCsvAsync(options.OutputCsv, output);
    }
}