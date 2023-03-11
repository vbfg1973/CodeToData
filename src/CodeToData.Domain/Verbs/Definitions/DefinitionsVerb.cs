using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeToData.Domain.Models;
using CodeToData.Domain.Services.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeToData.Domain.Verbs.Definitions
{
    public class DefinitionsVerb
    {
        public async Task Run(DefinitionsOptions options)
        {
            await Console.Error.WriteLineAsync(
                $"Compiling {options.Solution} for definitions");
            var compiler = new SolutionCompilerService(options.Solution);

            var definitions = new List<TypeDefinition>();

            foreach (var project in compiler.Projects)
            {
                var projectCompilation = compiler.GetCompilation(project.Name);
                var docs = compiler.GetDocuments(project.Name);

                await Console.Error.WriteLineAsync($"Examining {project.Name} - {projectCompilation.Language}");

                foreach (var doc in docs)
                    switch (projectCompilation.Language)
                    {
                        case "C#":
                            var r = await CSharpTypeDefinitions(doc);
                            definitions.AddRange(r.ToList());
                            break;

                        case "Visual Basic":
                            var vb = await VisualBasicTypeDefinitions(doc);
                            definitions.AddRange(vb.ToList());
                            break;

                        default: throw new ArgumentException("Unknown language");
                    }
            }

            await Utilities.SaveCsvAsync(options.OutputCsv, definitions);
        }

        private async Task<IEnumerable<TypeDefinition>> CSharpTypeDefinitions(Document doc)
        {
            var root = await (await doc.GetSyntaxTreeAsync())?.GetRootAsync()!;
            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

            var td = new List<TypeDefinition>();

            foreach (var cls in classes)
            {
                var ns = cls.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
                var nspace = ns == null ? "Unknown" : ns.Name.ToString();

                td.Add(new TypeDefinition(doc.FilePath, nspace, cls.Identifier.ToString()));
            }

            return td;
        }

        private async Task<IEnumerable<TypeDefinition>> VisualBasicTypeDefinitions(Document doc)
        {
            var root = await (await doc.GetSyntaxTreeAsync())?.GetRootAsync()!;
            var classes = root.DescendantNodes().OfType<ClassStatementSyntax>().ToList();

            var td = new List<TypeDefinition>();

            foreach (var cls in classes)
            {
                var ns = cls.Ancestors().OfType<NamespaceStatementSyntax>().FirstOrDefault();
                var nspace = ns == null ? "Unknown" : ns.Name.ToString();

                td.Add(new TypeDefinition(doc.FilePath, nspace, cls.Identifier.ToString()));
            }

            return td;
        }
    }
}