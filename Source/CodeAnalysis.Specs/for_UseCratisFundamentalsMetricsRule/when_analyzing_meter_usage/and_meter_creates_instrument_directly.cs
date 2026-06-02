// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_UseCratisFundamentalsMetricsRule.when_analyzing_meter_usage;

public class and_meter_creates_instrument_directly : given.a_usecratisfundamentalsmetricsrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
namespace System.Diagnostics.Metrics
{
    public sealed class Meter
    {
        public Meter(string name) { }
        public object CreateCounter<T>(string name) => null;
    }
}

class Sample
{
    void Create()
    {
        var meter = new System.Diagnostics.Metrics.Meter("test");
        meter.CreateCounter<int>("counter");
    }
}
""");

    [Fact] void should_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0026").ShouldBeTrue();
}
