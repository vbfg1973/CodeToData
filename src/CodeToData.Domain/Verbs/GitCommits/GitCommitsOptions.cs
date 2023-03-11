using CommandLine;

namespace CodeToData.Domain.Verbs.GitCommits
{
    [Verb("commits", HelpText = "Dump all commits from branch to csv")]
    public class GitCommitsOptions : BaseRepositoryOptions
    {
    }
}