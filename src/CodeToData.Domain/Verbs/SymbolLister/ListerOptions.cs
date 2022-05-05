using CommandLine;

namespace CodeToData.Domain.Verbs.SymbolLister;

[Verb("list", HelpText = "List all symbols available to each project in the solution")]
public class ListerOptions : BaseSolutionOptions
{
    [Option('n', "namespace", Required = false, HelpText = "Case insensitive filter on containing namespace of the symbols")]
    public string NamespaceFilter { get; set; }
}