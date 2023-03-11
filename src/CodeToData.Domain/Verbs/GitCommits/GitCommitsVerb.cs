using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeToData.Domain.Extensions;
using CodeToData.Domain.Models.GitData;
using LibGit2Sharp;

namespace CodeToData.Domain.Verbs.GitCommits
{
    public class GitCommitsVerb
    {
        public GitCommitsVerb()
        {
        }

        public async Task Run(GitCommitsOptions options)
        {
            var repo = new Repository(options.Repository);

            var commitEntries = new List<GitCommitEntry>();
            
            foreach (var commit in repo.Commits)
            {
                commitEntries.AddRange(commit.CommitEntries(repo));
            }

            await Utilities.SaveCsvAsync(options.OutputCsv, commitEntries);
            await Console.Error.WriteLineAsync($"Wrote {commitEntries.Count} commits");
        }
    }
}