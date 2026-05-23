// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs;

public partial class ArchitectureAnalyzerSpecs
{
    [Fact]
    public async Task ShouldNotWarnWhenUsingFundamentalsMeterAbstractions()
    {
        const string source = """
namespace Cratis.Metrics
{
    public interface IMeter<T>
    {
    }
}

static class HandlerMetrics
{
    public static void CountExecution(Cratis.Metrics.IMeter<Handler> meter)
    {
    }
}

class Handler
{
    readonly Cratis.Metrics.IMeter<Handler> _meter;

    public Handler(Cratis.Metrics.IMeter<Handler> meter)
    {
        _meter = meter;
    }

    public void Execute()
    {
        HandlerMetrics.CountExecution(_meter);
    }
}
""";

        var diagnostics = await Analyze(source);

        Assert.DoesNotContain(diagnostics, _ => _.Id == "CRARCH0026");
    }
}
