using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CodeToData.Domain.Services.CodeAnalysis;
using CommandLine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace CodeToData.Domain.Verbs.Repetition
{
    [Verb("repetition")]
    public class RepetitionOptions : BaseSolutionOptions
    {
    }

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
                .OrderByDescending(x => x.Count)
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
                            await CSharpWalker(doc);
                            break;

                        case "Visual Basic":
                            break;

                        default: throw new ArgumentException("Unknown language");
                    }
                }
            }
        }

        private async Task CSharpWalker(Document document)
        {
            if (document.TryGetSyntaxRoot(out var root))
            {
                foreach (var node in root.DescendantNodes())
                {
                    var codeSignature = await CSharpSignatureBuilder(node, document);

                    if (codeSignature.Descendants < 15)
                    {
                        continue;
                    }

                    if (codeSignature.CodeSnippet.StartsWith("using") && codeSignature.ChildNodeString.EndsWith("IdentifierName"))
                    {
                        continue;
                    }

                    // Make sure there's a hashset there to add to
                    if (!_dictionary.ContainsKey(codeSignature.Signature))
                    {
                        _dictionary[codeSignature.Signature] = new HashSet<CodeSignature>();
                    }

                    _dictionary[codeSignature.Signature].Add(codeSignature);
                }
            }
        }

        private static string CSharpNamespaceFinder(SyntaxNode syntaxNode)
        {
            return syntaxNode
                .Ancestors()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault()?
                .Name
                .ToString();
        }

        private async Task<CodeSignature> CSharpSignatureBuilder(SyntaxNode syntaxNode, Document document)
        {
            var ns = CSharpNamespaceFinder(syntaxNode);
            var childNodes = syntaxNode.DescendantNodes().ToList();

            var childString = string.Join("-", childNodes
                .Select(childNode => childNode.Kind().ToString()));

            var text = await document.GetTextAsync();
            var codeSnippet = text.ToString().Substring(syntaxNode.Span.Start, syntaxNode.Span.End - syntaxNode.Span.Start);

            return new CodeSignature
            {
                Signature = childString.GetHashCode(),
                Filename = document.FilePath,
                Project = document.Project.Name,
                Descendants = childNodes.Count,
                Namespace = string.IsNullOrEmpty(ns) ? "Unknown" : ns,
                Start = syntaxNode.Span.Start,
                End = syntaxNode.Span.End,
                ChildNodeString = childString,
                CodeSnippet = codeSnippet
            };
        }

        private record CodeSignature
        {
            public int Signature { get; set; }
            public string Project { get; set; }
            public string Namespace { get; set; }
            public string Filename { get; set; }
            public int Descendants { get; set; }
            public int Start { get; set; }
            public int End { get; set; }
            public string ChildNodeString { get; set; }
            public string CodeSnippet { get; set; }
        }
    }
}