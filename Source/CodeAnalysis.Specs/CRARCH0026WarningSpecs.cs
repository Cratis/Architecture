// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs;

public partial class ArchitectureAnalyzerSpecs
{
    [Fact]
    public async Task ShouldWarnForDirectMeterInstrumentCreation()
    {
        const string source = """
using System.Diagnostics.Metrics;

class Handler
{
    readonly Meter _meter = new("MyService");

    public void Execute()
    {
        var counter = _meter.CreateCounter<int>("my_counter");
        counter.Add(1);
    }
}
""";

        var diagnostics = await Analyze(source);

        Assert.Contains(diagnostics, _ => _.Id == "CRARCH0026");
    }
}
