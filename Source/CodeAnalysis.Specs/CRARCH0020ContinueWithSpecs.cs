namespace Cratis.Architecture.CodeAnalysis.Specs;

public partial class ArchitectureAnalyzerSpecs
{
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
}
