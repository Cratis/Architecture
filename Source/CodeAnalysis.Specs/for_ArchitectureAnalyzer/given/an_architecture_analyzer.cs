// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Specifications;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_ArchitectureAnalyzer.given;

public class an_architecture_analyzer : Specification
{
    protected IReadOnlyList<Diagnostic> _diagnostics = [];

    protected static async Task<IReadOnlyList<Diagnostic>> analyze(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, path: "/tmp/Source/CodeAnalysis/TestFile.cs");
        var compilation = CSharpCompilation.Create(
            assemblyName: "AnalyzerTests",
            syntaxTrees: [syntaxTree],
            references: get_metadata_references(),
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var analyzer = new ArchitectureAnalyzer();
        var diagnostics = await compilation.WithAnalyzers([analyzer]).GetAnalyzerDiagnosticsAsync().ConfigureAwait(false);
        return diagnostics;
    }

    static IReadOnlyList<MetadataReference> get_metadata_references()
    {
        var trustedAssemblies = (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string)?.Split(Path.PathSeparator)
                               ?? [];
        return trustedAssemblies.Select(path => MetadataReference.CreateFromFile(path)).ToList();
    }
}
