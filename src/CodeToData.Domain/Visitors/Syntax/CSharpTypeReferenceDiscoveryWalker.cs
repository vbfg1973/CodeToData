using System.Collections.Generic;
using CodeToData.Domain.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeToData.Domain.Visitors.Syntax
{
    public class CSharpTypeReferenceDiscoveryWalker : CSharpSyntaxWalker
    {
        private readonly Document _document;
        private readonly SemanticModel _model;
        private readonly SyntaxTree _tree;

        public CSharpTypeReferenceDiscoveryWalker(Compilation compilation, Document document)
        {
            _document = document;
            _tree = document.GetSyntaxTreeAsync().Result;
            _model = compilation.GetSemanticModel(_tree);
            Types = new List<DiscoveredType>();
            Visit();
        }

        public List<DiscoveredType> Types { get; set; }

        private void Visit()
        {
            Visit(_tree.GetRoot());
        }

        public override void Visit(SyntaxNode? node)
        {
            Types.AddRange(IdentifyType(node));

            base.Visit(node);
        }

        private IEnumerable<DiscoveredType> IdentifyType(SyntaxNode node)
        {
            var identifiedType = _model.GetTypeInfo(node);
            var convertedType = identifiedType.ConvertedType;

            if (convertedType is not { SpecialType: SpecialType.None })
            {
                yield break;
            }

            var assembly = GetAssembly(convertedType);
            var ns = GetNameSpace(convertedType);

            if (string.IsNullOrEmpty(assembly) || string.IsNullOrEmpty(ns))
            {
                yield break;
            }

            yield return new DiscoveredType
            {
                Name = convertedType.Name,
                SourceAssembly = assembly,
                SourceNamespace = ns,
                TypeKind = convertedType.TypeKind.ToString(),
                DocumentName = _document.FilePath,
                AssemblyName = _document.Project.AssemblyName,
                StartPosition = node.Span.Start,
                EndPosition = node.Span.End
            };
        }

        private IEnumerable<DiscoveredType> DiscoveredType(SyntaxNode node)
        {
            var symbolInfo = _model.GetSymbolInfo(node);
            var identifiedType = _model.GetTypeInfo(node);
            var convertedType = identifiedType.ConvertedType;

            if (convertedType is not { SpecialType: SpecialType.None })
            {
                yield break;
            }

            var assembly = string.Empty;
            var ns = string.Empty;

            try
            {
                assembly = GetAssembly(symbolInfo.Symbol);
                ns = GetNameSpace(symbolInfo.Symbol);
            }

            catch
            {
                //
            }

            if (string.IsNullOrEmpty(assembly) || string.IsNullOrEmpty(ns))
            {
                yield break;
            }

            yield return new DiscoveredType
            {
                Name = convertedType.Name,
                SourceAssembly = assembly,
                SourceNamespace = ns,
                TypeKind = convertedType.TypeKind.ToString(),
                DocumentName = _document.FilePath,
                AssemblyName = _document.Project.AssemblyName,
                StartPosition = node.Span.Start,
                EndPosition = node.Span.End
            };
        }

        private string GetNameSpace(ITypeSymbol typeSymbol)
        {
            return typeSymbol.ContainingNamespace != null ? typeSymbol.ContainingNamespace.ToDisplayString() : string.Empty;
        }

        private string GetNameSpace(ISymbol typeSymbol)
        {
            return typeSymbol.ContainingNamespace != null ? typeSymbol.ContainingNamespace.ToDisplayString() : string.Empty;
        }

        private string GetAssembly(ITypeSymbol typeSymbol)
        {
            return typeSymbol.ContainingAssembly != null ? typeSymbol.ContainingAssembly.Name : string.Empty;
        }

        private string GetAssembly(ISymbol typeSymbol)
        {
            return typeSymbol.ContainingAssembly != null ? typeSymbol.ContainingAssembly.Name : string.Empty;
        }

        private string NodeToString(SyntaxNode node)
        {
            var str = node.ToFullString();
            return str.Length > 40 ? "TOO LONG" : str.ReplaceLineEndings("\t");
        }
    }
}