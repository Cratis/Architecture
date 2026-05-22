namespace Cratis.Architecture.CodeAnalysis.Specs;

public partial class ArchitectureAnalyzerSpecs
{
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
}
