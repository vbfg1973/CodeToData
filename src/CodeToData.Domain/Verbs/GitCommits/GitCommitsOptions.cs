using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeToData.Domain.Extensions;
using CodeToData.Domain.Models.GitData;
using CommandLine;
using LibGit2Sharp;

namespace CodeToData.Domain.Verbs.GitCommits
{
    [Verb("commits")]
    public class GitCommitsOptions : BaseRepositoryOptions
    {
    }

    public class GitCommitsVerb
    {
        public GitCommitsVerb()
        {
        }

        public async Task Run(GitCommitsOptions options)
        {
            var repo = new Repository(options.Repository);

            foreach (var commit in repo.Commits)
            {
                var commitMeta = commit.CommitMeta();

                if (commit.Parents.Count() != 1) continue; // More than one parent == merge. GTFO.
                
                var patch = repo.Diff.Compare<Patch>(commit.Parents.Single().Tree, commit.Tree);

                Console.WriteLine(commitMeta);
                foreach (var p in patch)
                {
                    Console.WriteLine("\t{0}", p.Path);
                }

                Console.WriteLine();
            }
        }
    }
}