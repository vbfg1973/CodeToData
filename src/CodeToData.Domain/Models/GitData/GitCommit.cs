using System;
using System.Text;
using LibGit2Sharp;

namespace CodeToData.Domain.Models.GitData
{
    public record CommitMeta
    {
        public CommitMeta(Commit commit)
        {
            Id = commit.Id.ToString();
            Author = commit.Author.Email;
            DateTime = commit.Author.When.DateTime;
        }
        
        public string Id { get; }
        public DateTime DateTime { get; set; }
        public string Author { get; set; }
    }
}