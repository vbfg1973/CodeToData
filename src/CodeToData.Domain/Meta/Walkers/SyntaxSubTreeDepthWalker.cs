using System;
using System.Collections.Generic;
using System.Linq;
using CodeToData.Domain.Meta.Data;
using Microsoft.CodeAnalysis;

namespace CodeToData.Domain.Meta.Walkers
{
    public class SyntaxSubTreeDepthWalker : SyntaxWalker
    {
        private int _depth;
        private List<int> _lDepths = new();

        public SyntaxSubTreeDepth Walk(SyntaxNode syntaxNode)
        {
            Visit(syntaxNode);

            var nodeCount = syntaxNode.DescendantNodes().Count();
            var medianDepth = (_lDepths.OrderBy(x => x)).ToList()[Convert.ToInt32(nodeCount / 2)];
            var meanDepth = Convert.ToInt32(_lDepths.Sum() / _lDepths.Count);
            var maxDepth = Convert.ToInt32(_lDepths.Max());

            return new SyntaxSubTreeDepth(maxDepth: maxDepth, meanDepth: meanDepth, nodeCount: nodeCount, medianDepth: medianDepth);
        }

        public override void Visit(SyntaxNode? node)
        {
            _depth++;
            _lDepths.Add(_depth);
            base.Visit(node);
            _depth--;
        }
    }
}