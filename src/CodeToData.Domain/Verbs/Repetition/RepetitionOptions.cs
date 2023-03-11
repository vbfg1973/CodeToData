using System.Collections.Concurrent;
using CommandLine;

namespace CodeToData.Domain.Verbs.Repetition
{
    [Verb("repetition", HelpText = "Walk all ASTs in the solution and detect repeating patterns in the code")]
    public class RepetitionOptions : BaseSolutionOptions
    {
    }
}