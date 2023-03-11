using System;
using System.Collections.Generic;
using System.Linq;
using CodeToData.Domain.Meta.Data;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeToData.Domain.Meta
{
    public static class MetaDataExtensions
    {
        public static IEnumerable<ClassDeclarationSyntax> ClassDeclarations(this Document document)
        {
            var root = document.GetSyntaxRootAsync().Result;
            return root.DescendantNodes().OfType<ClassDeclarationSyntax>();
        }

        public static IEnumerable<MethodDeclarationSyntax> MethodDeclarations(this Document document)
        {
            var root = document.GetSyntaxRootAsync().Result;
            return root.DescendantNodes().OfType<MethodDeclarationSyntax>();
        }

        public static IEnumerable<ClassBlockSyntax> ClassBlocks(this Document document)
        {
            var root = document.GetSyntaxRootAsync().Result;
            return root.DescendantNodes().OfType<ClassBlockSyntax>();
        }

        public static IEnumerable<MethodBlockSyntax> MethodBlocks(this Document document)
        {
            var root = document.GetSyntaxRootAsync().Result;
            return root.DescendantNodes().OfType<MethodBlockSyntax>();
        }

        public static LineSpan LineSpan(this Document document, TextSpan span)
        {
            if (!document.TryGetText(out var sourceText)) return new LineSpan(int.MinValue, int.MinValue);

            var matchingLines = sourceText.Lines.Where(x => x.SpanIncludingLineBreak.IntersectsWith(span)).ToList();

            return matchingLines.Count switch
            {
                1 => new LineSpan(matchingLines.First().LineNumber),
                > 1 => new LineSpan(matchingLines.First().LineNumber, matchingLines.Last().LineNumber),
                _ => throw new Exception()
            };
        }

        public static Indentation Indentation(this Document document, LineSpan lineSpan)
        {
            var sourceText = document.GetTextAsync().Result;
            var textLines = sourceText.Lines.Where(x => lineSpan.IntersectsWith(x.LineNumber)).ToArray();

            
            
            var texts = textLines.Select(textLine => textLine
                .Text
                .ToString());
                
            //     .TakeWhile(char.IsWhiteSpace)
            //     .Count()
            // ).ToList();

            var count = indentations.Count;
            var mean = Convert.ToInt32(indentations.Sum() / count);
            var max = indentations.Max();
            var median = (indentations.OrderBy(x => x)).ToList()[Convert.ToInt32(count / 2)];

            return new Indentation(median, max, mean);
        }
    }
}