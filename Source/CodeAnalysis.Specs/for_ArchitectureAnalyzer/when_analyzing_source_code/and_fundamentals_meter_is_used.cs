// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.for_ArchitectureAnalyzer.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_ArchitectureAnalyzer.when_analyzing_source_code;

public class and_fundamentals_meter_is_used : given.an_architecture_analyzer
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
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
""");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0026").ShouldBeFalse();
}
