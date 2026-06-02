// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_UseCratisFundamentalsTracesRule.when_analyzing_activity_source_usage;

public class and_activitysource_startactivity_is_called_directly : given.a_usecratisfundamentalstracesrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
namespace System.Diagnostics
{
    public sealed class ActivitySource
    {
        public ActivitySource(string name) { }
        public object StartActivity() => null;
    }
}

class Sample
{
    void Start()
    {
        var source = new System.Diagnostics.ActivitySource("test");
        source.StartActivity();
    }
}
""");

    [Fact] void should_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0025").ShouldBeTrue();
}
