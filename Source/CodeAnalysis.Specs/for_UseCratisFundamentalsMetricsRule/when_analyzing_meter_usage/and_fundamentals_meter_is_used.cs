// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_UseCratisFundamentalsMetricsRule.when_analyzing_meter_usage;

public class and_fundamentals_meter_is_used : given.a_usecratisfundamentalsmetricsrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
namespace Cratis.Fundamentals.Observability
{
    public static class MeterExtensions { }
}

class Sample
{
    void Use()
    {
        var ext = typeof(Cratis.Fundamentals.Observability.MeterExtensions);
    }
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0026").ShouldBeFalse();
}
