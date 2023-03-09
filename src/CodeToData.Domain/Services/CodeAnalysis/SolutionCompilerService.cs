using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeToData.Domain.Visitors.Semantics;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace CodeToData.Domain.Services.CodeAnalysis
{
    public class SolutionCompilerService
    {
        private readonly Dictionary<string, Compilation> _compilations = new();
        private readonly Dictionary<string, Project> _projects = new();
        private readonly Dictionary<string, INamedTypeSymbol> _symbols = new();

        public SolutionCompilerService(string solutionPath)
        {
            SolutionPath = solutionPath;

            MSBuildLocator.RegisterDefaults();
            using var workspace = MSBuildWorkspace.Create();
            workspace.WorkspaceFailed += (o, e) =>
            {
                Console.Error.WriteLine($"MSBuild {e.Diagnostic.Kind}: {e.Diagnostic.Message}");
                Console.Error.WriteLine();
            };

            Solution = workspace.OpenSolutionAsync(SolutionPath).Result;
            BuildIt().Wait();
        }

        public string SolutionPath { get; }

        public Solution Solution { get; }
        public IEnumerable<Project> Projects => _projects.Values;
        public IEnumerable<Compilation> Compilations => _compilations.Values;
        public IEnumerable<Document> Documents => Projects.SelectMany(project => project.Documents);

        public Compilation GetCompilation(string projectName)
        {
            return _compilations[projectName];
        }

        public IEnumerable<Document> GetDocuments(string projectName)
        {
            return _projects[projectName].Documents;
        }

        private async Task BuildIt()
        {
            foreach (var project in Solution.Projects)
            {
                await Console.Error.WriteLineAsync($"Building: {project.Name}");
                var compilation = await project.GetCompilationAsync();

                if (compilation == null)
                {
                    continue;
                }

                var symbolWalker = new NamedTypeDiscoveryVisitor();
                symbolWalker.Visit(compilation.GlobalNamespace);

                foreach (var symbol in symbolWalker.Symbols)
                {
                    _symbols[$"{symbol.ContainingAssembly.Name}_{symbol.Name}"] = symbol;
                }

                _compilations[project.Name] = compilation;
                _projects[project.Name] = project;
            }
        }
    }
}