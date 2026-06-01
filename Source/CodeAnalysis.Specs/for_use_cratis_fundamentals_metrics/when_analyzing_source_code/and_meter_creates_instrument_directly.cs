// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_use_cratis_fundamentals_metrics.when_analyzing_source_code;

public class and_meter_creates_instrument_directly : given.an_architecture_analyzer
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
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
""");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0026").ShouldBeTrue();
}
