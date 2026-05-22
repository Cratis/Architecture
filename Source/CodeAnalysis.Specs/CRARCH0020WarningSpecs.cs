namespace Cratis.Architecture.CodeAnalysis.Specs;

public partial class ArchitectureAnalyzerSpecs
{
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
}
