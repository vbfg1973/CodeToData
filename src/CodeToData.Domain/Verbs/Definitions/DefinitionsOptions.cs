using CommandLine;

namespace CodeToData.Domain.Verbs.Definitions
{
    [Verb("definitions", HelpText = "Find all class definitions and list their source")]
    public sealed class DefinitionsOptions : BaseSolutionOptions
    {
    }
}