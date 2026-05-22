namespace Cratis.Architecture.CodeAnalysis.Specs;

public partial class ArchitectureAnalyzerSpecs
{
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
}
