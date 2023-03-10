using System.Collections.Generic;
using System.Linq;
using CodeToData.Domain.Models;

namespace CodeToData.Domain.Extensions
{
    public static class CodeSignatureFiltering
    {
        public static bool IsInsideAnother(this IEnumerable<CodeSignature> codeSignatures, CodeSignature codeSignature)
        {
            return codeSignatures.Any(cs => codeSignature.Start > cs.Start && codeSignature.End <= cs.End);
        }
    }
}