using CodeToData.Domain.Models.GitData;
using LibGit2Sharp;

namespace CodeToData.Domain.Extensions
{
    public static class GitExtensions
    {
        public static CommitMeta CommitMeta(this Commit commit)
        {
            return new CommitMeta(commit);
        } 
    }
}