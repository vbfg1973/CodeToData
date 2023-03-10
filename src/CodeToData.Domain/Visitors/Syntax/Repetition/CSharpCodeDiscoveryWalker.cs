using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeToData.Domain.Extensions;
using CodeToData.Domain.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeToData.Domain.Visitors.Syntax.Repetition
{
    public class CSharpCodeDiscoveryWalker : CSharpSyntaxWalker
    {
        private readonly Compilation _compilation;
        private readonly Document _document;
        private readonly SemanticModel _model;
        private readonly SyntaxTree _tree;

        public List<CodeSignature> CodeSignatures { get; } = new List<CodeSignature>();

        private bool IsValid => _compilation != null &&
                                _document != null &&
                                _model != null &&
                                _tree != null;

        public CSharpCodeDiscoveryWalker(Compilation compilation, Document document)
        {
            _compilation = compilation;
            _document = document;
            _tree = document.GetSyntaxTreeAsync().Result;

            if (_tree != null)
            {
                _model = compilation.GetSemanticModel(_tree);
            }

            if (IsValid)
            {
                Visit();
            }
        }

        private void Visit()
        {
            Visit(_tree.GetRoot());
        }

        public override void Visit(SyntaxNode? node)
        {
            var signature = SignatureBuilder(node, _document).Result;

            CodeSignatures.Add(signature);

            base.Visit(node);
        }

        private async Task<CodeSignature> SignatureBuilder(SyntaxNode syntaxNode, Document document)
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

        private static string CSharpNamespaceFinder(SyntaxNode syntaxNode)
        {
            return syntaxNode
                .Ancestors()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault()?
                .Name
                .ToString();
        }
    }
}