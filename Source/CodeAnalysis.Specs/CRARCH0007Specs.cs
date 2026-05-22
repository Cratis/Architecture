// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs;

public partial class ArchitectureAnalyzerSpecs
{
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
}
