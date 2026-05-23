// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs;

public partial class ArchitectureAnalyzerSpecs
{
    [Fact]
    public async Task ShouldWarnForDirectActivitySourceStartActivityUsage()
    {
        const string source = """
using System.Diagnostics;

class Handler
{
    readonly ActivitySource _source = new("MyService");

    public void Execute()
    {
        using var scope = _source.StartActivity("process");
    }
}
""";

        var diagnostics = await Analyze(source);

        Assert.Contains(diagnostics, _ => _.Id == "CRARCH0025");
    }
}
