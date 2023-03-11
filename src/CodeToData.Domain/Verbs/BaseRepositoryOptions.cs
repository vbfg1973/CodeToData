using CommandLine;

namespace CodeToData.Domain.Verbs
{
    public abstract class BaseRepositoryOptions
    {
        [Option('r', "repository", Required = true, HelpText = "Path to repo")]
        public string Repository { get; set; }

        [Option('o', "output", Required = true, HelpText = "Path to output csv")]
        public string OutputCsv { get; set; }
    }
}