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

### list

    CodeToData list -s <solution_file_path> -o <output_csv> -n <optional_namespace_filter>

This lists every type available to the solution whether used or not.

The namespace filter is a simple case insensitive filter.

#### Output schema:

* SymbolName - Name of the discovered type
* ContainingAssembly - Source assembly of the discovered type
* ContainingNamespace - Containing namespace of the discovered type
* TypeKind - The symbol type (Class, Interface, Enum, Record, Struct, Delegate)

### find

    CodeToData find -s <solution_file_path> -o <output_csv> -n <optional_namespace_filter>

More usefully, discovers every type available to the project and lists the usages.

#### Output schema:

* SymbolName - Name of the discovered type
* ContainingAssembly - Source assembly of the discovered type
* ContainingNamespace - Containing namespace of the discovered type
* TypeKind - The symbol type (Class, Interface, Enum, Record, Struct, Delegate)
* Project - The Project in the solution where the usage was found
* Document - The full path to the file where found
* StartPosition - Number of characters from the beginning of the file
* EndPosition - Number of characters from the beginning of the file