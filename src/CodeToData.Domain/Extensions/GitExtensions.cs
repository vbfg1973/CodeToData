using System;
using System.Collections.Generic;
using System.Linq;
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
        
        public static IEnumerable<GitCommitEntry> CommitEntries(this Commit commit, Repository repo)
        {
            // If more than one parent then a merge - we're getting the commit data direct from the commits
            if (commit.Parents.Count() != 1)
            {
                yield break;
            }
                
            var patches = repo.Diff.Compare<Patch>(commit.Parents.Single().Tree, commit.Tree);

            foreach (var patch in patches)
            {
                yield return new GitCommitEntry(commit, patch);
            }   
        } 
    }
}