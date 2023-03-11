using System;
using System.Threading.Tasks;
using CodeToData.Domain.Meta;
using CodeToData.Domain.Meta.Walkers;
using CodeToData.Domain.Services.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace CodeToData.Domain.Verbs.ClassMeta
{
    public class ClassMetaVerb
    {
        private readonly ILogger<ClassMetaVerb> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public ClassMetaVerb(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<ClassMetaVerb>();
        }

        public async Task Run(ClassMetaOptions options)
        {
            await Console.Error.WriteLineAsync(
                $"Compiling {options.Solution} for definitions");
            var compilerService = new SolutionCompilerService(options.Solution);
            var nodeDepthWalker = new SyntaxSubTreeDepthWalker();
            
            foreach (var p in compilerService.Projects)
            foreach (var d in p.Documents)
                switch (p.Language)
                {
                    case "C#":
                        var a = d.ClassDeclarations();

                        foreach (var cls in a)
                        {
                            var lineSpan = d.LineSpan(cls.Span);

                            var nodeDepth = nodeDepthWalker.Walk(cls);

                            var indentation = d.Indentation(lineSpan);

                            Console.WriteLine(
                                $"{d.FilePath} {cls.Span.Start} - {cls.Span.End} {indentation} {lineSpan} {nodeDepth}");
                        }

                        break;

                    case "Visual Basic":
                        break;

                    default: throw new ArgumentException("Unknown language");
                }
        }
    }
}