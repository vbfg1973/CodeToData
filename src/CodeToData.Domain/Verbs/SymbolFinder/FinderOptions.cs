using CommandLine;

namespace CodeToData.Domain.Verbs.SymbolFinder;

[Verb("find", HelpText = "List locations of all NamedType symbol references in the solution")]
public class FinderOptions : BaseSolutionOptions
{
    [Option('n', "namespace", Required = false, HelpText = "Case insensitive filter on containing namespace of the symbols")]
    public string NamespaceFilter { get; set; }
}