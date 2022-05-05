using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeToData.Domain.Models;
using CodeToData.Domain.Services.CodeAnalysis;
using CodeToData.Domain.Visitors.Semantics;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace CodeToData.Domain.Verbs.SymbolLister;

public class ListerVerb
{
    private readonly ILoggerFactory _loggerFactory;
    private ILogger<ListerVerb> _logger;

    public ListerVerb(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<ListerVerb>();
    }

    public async Task Run(ListerOptions options)
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

        await Utilities.SaveCsvAsync(options.OutputCsv, symbols);
    }
}