// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs;

public partial class ArchitectureAnalyzerSpecs
{
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
}
