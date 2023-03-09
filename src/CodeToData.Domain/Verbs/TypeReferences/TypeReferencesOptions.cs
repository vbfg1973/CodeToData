using CommandLine;

namespace CodeToData.Domain.Verbs.TypeReferences
{
    [Verb("references", HelpText = "List all type references in each project in the solution")]
    public class TypeReferencesOptions : BaseSolutionOptions
    {
        [Option('n', "namespace", Required = false,
            HelpText = "Filter on containing namespace of the types")]
        public string SourceNamespaceFilter { get; set; }

        [Option('a', "assembly", Required = false,
            HelpText = "Filter on containing assembly of the types")]
        public string SourceAssemblyFilter { get; set; }

        [Option('i', "caseInsensitive", Default = true, HelpText = "Toggles case insensitivity of filters")]
        public bool CaseInsensitive { get; set; }

        [Option('g', "globalNamespace", Default = false, HelpText = "Ignores types from global namespace unless turned on")]
        public bool GlobalNamespace { get; set; }

        [Option('m', "mscorlib", Default = false, HelpText = "Ignores types from mscorlib assembly (Guids, IEnumerable, et al) unless turned on")]
        public bool MSCorlib { get; set; }

        [Option('n', "netstandard", Default = false, HelpText = "Ignores types from netstandard assembly")]
        public bool NetStandard { get; set; }
    }
}