using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Specs;

public class ArchitectureAnalyzerSpecs
{
    [Fact]
    public async Task ShouldWarnForExceptionSuffix()
    {
        const string source = """
using System;

class AuthorNotFoundException : Exception
{
}
""";

        var diagnostics = await Analyze(source);

        Assert.Contains(diagnostics, _ => _.Id == "CRARCH0001");
    }

    [Fact]
    public async Task ShouldWarnForIServiceProviderInjection()
    {
        const string source = """
using System;

class Handler
{
    public Handler(IServiceProvider provider)
    {
    }
}
""";

        var diagnostics = await Analyze(source);

        Assert.Contains(diagnostics, _ => _.Id == "CRARCH0007");
    }

    [Fact]
    public async Task ShouldWarnForConcreteInjectionThatIsNotReadModel()
    {
        const string source = """
class Dependency
{
}

class Handler
{
    public Handler(Dependency dependency)
    {
    }
}
""";

        var diagnostics = await Analyze(source);

        Assert.Contains(diagnostics, _ => _.Id == "CRARCH0018");
    }

    [Fact]
    public async Task ShouldNotWarnForConcreteInjectionWhenTypeIsMarkedAsReadModel()
    {
        const string source = """
using System;

[AttributeUsage(AttributeTargets.Class)]
class ReadModelAttribute : Attribute
{
}

[ReadModel]
class CustomerReadModel
{
}

class Handler
{
    public Handler(CustomerReadModel customer)
    {
    }
}
""";

        var diagnostics = await Analyze(source);

        Assert.DoesNotContain(diagnostics, _ => _.Id == "CRARCH0018");
    }

    [Fact]
    public async Task ShouldWarnForAsyncSuffixWithoutSynchronousCounterpart()
    {
        const string source = """
using System.Threading.Tasks;

class Handler
{
    public Task ProcessAsync() => Task.CompletedTask;
}
""";

        var diagnostics = await Analyze(source);

        Assert.Contains(diagnostics, _ => _.Id == "CRARCH0019");
    }

    [Fact]
    public async Task ShouldNotWarnForAsyncSuffixWhenSynchronousCounterpartExists()
    {
        const string source = """
using System.Threading.Tasks;

class Handler
{
    public void Process()
    {
    }

    public Task ProcessAsync() => Task.CompletedTask;
}
""";

        var diagnostics = await Analyze(source);

        Assert.DoesNotContain(diagnostics, _ => _.Id == "CRARCH0019");
    }

    [Fact]
    public async Task ShouldWarnForUnhandledAsyncCall()
    {
        const string source = """
using System.Threading.Tasks;

class Handler
{
    Task ProcessAsync() => Task.CompletedTask;

    void Execute()
    {
        ProcessAsync();
    }
}
""";

        var diagnostics = await Analyze(source);

        Assert.Contains(diagnostics, _ => _.Id == "CRARCH0020");
    }

    [Fact]
    public async Task ShouldNotWarnForContinuedAsyncCall()
    {
        const string source = """
using System;
using System.Threading.Tasks;

class Handler
{
    Task ProcessAsync() => Task.CompletedTask;

    void Execute()
    {
        ProcessAsync().ContinueWith(_ => Console.WriteLine("done"));
    }
}
""";

        var diagnostics = await Analyze(source);

        Assert.DoesNotContain(diagnostics, _ => _.Id == "CRARCH0020");
    }

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
