using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Specs;

public partial class ArchitectureAnalyzerSpecs
{
    static async Task<IReadOnlyList<Diagnostic>> Analyze(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, path: "/tmp/Source/CodeAnalysis/TestFile.cs");
        var compilation = CSharpCompilation.Create(
            assemblyName: "AnalyzerTests",
            syntaxTrees: [syntaxTree],
            references: GetMetadataReferences(),
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var analyzer = new ArchitectureAnalyzer();
        var diagnostics = await compilation.WithAnalyzers([analyzer]).GetAnalyzerDiagnosticsAsync();
        return diagnostics;
    }

    static IReadOnlyList<MetadataReference> GetMetadataReferences()
    {
        var trustedAssemblies = (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string)?.Split(Path.PathSeparator)
                               ?? [];
        return trustedAssemblies.Select(path => MetadataReference.CreateFromFile(path)).ToList();
    }
}
