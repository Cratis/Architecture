namespace Cratis.Architecture.CodeAnalysis.Specs;

public partial class ArchitectureAnalyzerSpecs
{
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
}
