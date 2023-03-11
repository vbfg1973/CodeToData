using System;
using LibGit2Sharp;

namespace CodeToData.Domain.Models.GitData
{
    public record GitCommitEntry
    {
        public string Id { get; }
        public DateTime DateTime { get; }
        public string Author { get; }
        public string Path { get; }
        public string OldPath { get; }
        public bool PathChanges => Path != OldPath;
        public int LinesAdded { get; }
        public int LinesDeleted { get; }

        public GitCommitEntry(Commit commit, PatchEntryChanges patchEntryChanges)
        {
            Id = commit.Id.ToString();
            Author = commit.Author.Email;
            DateTime = commit.Author.When.DateTime;
            OldPath = patchEntryChanges.OldPath;
            Path = patchEntryChanges.Path;
            LinesAdded = patchEntryChanges.LinesAdded;
            LinesDeleted = patchEntryChanges.LinesDeleted;
        }
    }
}