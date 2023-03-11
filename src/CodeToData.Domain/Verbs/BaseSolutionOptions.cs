using CommandLine;

namespace CodeToData.Domain.Verbs
{
    public abstract class BaseSolutionOptions
    {
        [Option('s', "solution", Required = true, HelpText = "Path to solution file")]
        public string Solution { get; set; }

        [Option('o', "output", Required = true, HelpText = "Path to output csv")]
        public string OutputCsv { get; set; }
    }
    
    public abstract class BaseRepositoryOptions
    {
        [Option('r', "repository", Required = true, HelpText = "Path to repo")]
        public string Repository { get; set; }
    }
}