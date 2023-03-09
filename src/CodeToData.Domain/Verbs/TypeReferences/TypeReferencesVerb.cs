using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeToData.Domain.Extensions;
using CodeToData.Domain.Models;
using CodeToData.Domain.Services.CodeAnalysis;
using CodeToData.Domain.Visitors.Syntax;
using Microsoft.Extensions.Logging;

namespace CodeToData.Domain.Verbs.TypeReferences
{
    public class TypeReferencesVerb
    {
        private readonly ILoggerFactory _loggerFactory;
        private ILogger<TypeReferencesVerb> _logger;

        public TypeReferencesVerb(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<TypeReferencesVerb>();
        }

        public async Task Run(TypeReferencesOptions options)
        {
            await Console.Error.WriteLineAsync(
                $"Compiling {options.Solution}");
            var compiler = new SolutionCompilerService(options.Solution);

            var types = new List<DiscoveredType>();

            foreach (var project in compiler.Projects)
            {
                var projectCompilation = compiler.GetCompilation(project.Name);
                var docs = compiler.GetDocuments(project.Name);

                await Console.Error.WriteLineAsync($"Examining {project.Name} - {projectCompilation.Language}");

                foreach (var doc in docs)
                {
                    switch (projectCompilation.Language)
                    {
                        case "C#":
                            CSharpTypeReferenceDiscoveryWalker csharpwalker = new(projectCompilation, doc);
                            types.AddRange(FilterDiscoveredTypes(csharpwalker.Types, options));
                            break;

                        case "Visual Basic":
                            VisualBasicTypeReferenceDiscoveryWalker vbwalker = new(projectCompilation, doc);
                            types.AddRange(FilterDiscoveredTypes(vbwalker.Types, options));
                            break;

                        default: throw new ArgumentException("Unknown language");
                    }
                }
            }

            await Utilities.SaveCsvAsync(options.OutputCsv, types);
        }

        private IEnumerable<DiscoveredType> FilterDiscoveredTypes(IEnumerable<DiscoveredType> types,
            TypeReferencesOptions options)
        {
            if (!string.IsNullOrEmpty(options.SourceAssemblyFilter))
            {
                types = types.FilterBySourceAssembly(options.SourceAssemblyFilter, options.CaseInsensitive);
            }

            if (!string.IsNullOrEmpty(options.SourceNamespaceFilter))
            {
                types = types.FilterBySourceNamespace(options.SourceNamespaceFilter, options.CaseInsensitive);
            }

            if (!options.GlobalNamespace)
            {
                types = types.InvertFilterBySourceNamespace("global namespace");
            }

            if (!options.MSCorlib)
            {
                types = types.InvertFilterBySourceAssembly("mscorlib");
            }

            if (!options.MSCorlib)
            {
                types = types.InvertFilterBySourceAssembly("netstandard");
            }

            return types;
        }
    }
}