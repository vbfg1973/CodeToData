# CodeToData

Discovers references to code symbols in dotnet projects. The purpose is to quickly get an understanding of code and
project dependencies sprayed throughout a whole solution.

It builds the whole solution and uses the compiler to interrogate the semantic model rather than parsing the syntax.
This allows for types instantiated with 'var' to be known, but only what is known at compile time:

    var unknownType = factory.GetSomeType(someData)

If the output of GetSomeType() is an interface IUnknownType then it will detect a usage of IUnknownType. However, the
concrete implementation of IUnknownType won't be known until runtime. For that you'll need a debugger.

## Constraints

It should work if you have the build infrastructure for the thing you're trying to build on the machine where you're
running it. For me it works on my Win10 laptop with all the versions of Framework I have installed, and all the Net Core
3.1, 5 and 6.

It has also worked with all those core versions on my Linux desktop. I have not tried it with Mono on Linux and don't
care to know.

## Running it

You **must** build it in order to run it. The verb-like command-line interface does not play nice with ```dotnet run```.
Build it, run the executable or ```dotnet exec <path_to_dll> verb -s <solution_file_path> -o <output_csv>```

## Usages:

### definitions

    CodeToData definitions -s <solution_file_path> -o <output_csv>

Lists all class definitions in the code. I only added this so that any analysis about the files themselves, for example through git, will give an idea of which types we're talking about when talking about a file.

### references

    CodeToData references -s <solution_file_path> -o <output_csv>

Walks the ASTs looking for definitions of types contained within the solution. It will then walk the semantic model looking for references to those types and enumerating them with context into the output CSV.

### repetition

    CodeToData references -s <solution_file_path> -o s > output.json

__Note that the command line is slightly different. Not entirely sure what I want the output of this to look like atm so for now it's dumping a series of JSON arrays, one for each pattern family. The patterns with the largest number of descendants are at the top of the output best captured atm by redirecting to a file.__

__Best way to use this at present is open the output in notepad++ and search for the filenames you're presently interested in. Features for filtering on this and other bases are coming soon.__

Walks the ASTs in the solution building textual descriptions of the subtree from each node. From this 'signature' somewhat reliable detection of repeating patterns in the code can be used. 

It finds too much. It finds things which appear not to be similar and may not be, but just happen to have a similar structure. However, if you know where your problem areas are likely to be found then it can be useful.

The key to finding your problem areas are: 

1) Find the most complex files by your favourite measure of complexity (cyclomatic or GTFO).
2) Those complex files which change often, as measurable with git.

Files which appear near the top of both lists are your high value targets. These are the files which are:

* difficult to read for a developer new to the project
* in need of remediation, hence all the activity
* in the most danger of bugs being introduced

The most profit exists in breaking these out into digestible chunks and removal of any and all repetition.

Look also for those files that often change at the same time. Architectural smells abound and you may need to figure these into refactoring plans devised from these methods.

### commits

    CodeToData commits -r <repository_path> -o <output.csv>

Lists files changed by a commit
