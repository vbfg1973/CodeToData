using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CodeToData.Domain.Models;
using CodeToData.Domain.Services.CodeAnalysis;
using CodeToData.Domain.Visitors.Syntax.Repetition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace CodeToData.Domain.Verbs.Repetition
{
    public class RepetitionVerb
    {
        private readonly Dictionary<int, HashSet<CodeSignature>> _dictionary = new();
        private readonly ILogger<RepetitionVerb> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public RepetitionVerb(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<RepetitionVerb>();
        }

        public async Task Run(RepetitionOptions options)
        {
            var compiler = new SolutionCompilerService(options.Solution);

            await ScanProjects(compiler);

            foreach (var key in _dictionary.Keys)
            {
                if (_dictionary[key].Count == 1)
                {
                    _dictionary.Remove(key);
                }

                else
                {
                    await Console.Error.WriteLineAsync($"Count={_dictionary[key].Count}\tHashCode={key}\tDescendantCount={_dictionary[key].First().Descendants}");
                }
            }

            var list = _dictionary
                .Values
                .OrderByDescending(x => x.First().Descendants)
                .ToList();

            foreach (var set in list)
            {
                Console.Error.WriteLine(set.Count);
                Console.WriteLine(JsonSerializer.Serialize(set, new JsonSerializerOptions()
                {
                    WriteIndented = true
                }));
            }
        }

        private async Task ScanProjects(SolutionCompilerService compiler)
        {
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
                            CSharpWalkerInvocation(projectCompilation, doc);

                            break;

                        case "Visual Basic":
                            break;

                        default: throw new ArgumentException("Unknown language");
                    }
                }
            }
        }

        private void CSharpWalkerInvocation(Compilation projectCompilation, Document doc)
        {
            var walker = new CSharpCodeDiscoveryWalker(projectCompilation, doc);

            foreach (var c in walker.CodeSignatures)
            {
                if (!_dictionary.ContainsKey(c.Signature))
                {
                    _dictionary[c.Signature] = new HashSet<CodeSignature>();
                }

                _dictionary[c.Signature].Add(c);
            }
        }
    }
}