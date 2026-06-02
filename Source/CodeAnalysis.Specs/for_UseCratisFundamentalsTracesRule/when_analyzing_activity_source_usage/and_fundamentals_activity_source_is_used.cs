// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_UseCratisFundamentalsTracesRule.when_analyzing_activity_source_usage;

public class and_fundamentals_activity_source_is_used : given.a_usecratisfundamentalstracesrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
namespace Cratis.Fundamentals.Observability
{
    public static class ActivitySourceExtensions { }
}

class Sample
{
    void Use()
    {
        var ext = typeof(Cratis.Fundamentals.Observability.ActivitySourceExtensions);
    }
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0025").ShouldBeFalse();
}
