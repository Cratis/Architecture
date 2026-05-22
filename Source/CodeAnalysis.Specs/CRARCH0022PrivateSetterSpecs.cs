// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs;

public partial class ArchitectureAnalyzerSpecs
{
    [Fact]
    public async Task ShouldNotWarnForPrivateSetterOnPublicProperty()
    {
        const string source = """
public class Handler
{
    public int Count { get; private set; }
}
""";

        var diagnostics = await Analyze(source);

        Assert.DoesNotContain(diagnostics, _ => _.Id == "CRARCH0022");
    }
}
