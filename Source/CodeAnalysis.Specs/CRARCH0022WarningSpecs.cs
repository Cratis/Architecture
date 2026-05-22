// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs;

public partial class ArchitectureAnalyzerSpecs
{
    [Fact]
    public async Task ShouldWarnForExplicitPrivateModifier()
    {
        const string source = """
class Handler
{
    private int _count;
}
""";

        var diagnostics = await Analyze(source);

        Assert.Contains(diagnostics, _ => _.Id == "CRARCH0022");
    }
}
