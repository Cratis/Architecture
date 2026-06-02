// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_ConstructorFanOutRule.when_analyzing_constructor_dependencies;

public class and_constructor_has_seven_or_fewer_dependencies : given.a_constructorfanoutrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
class Sample
{
    public Sample(int dep1, int dep2, int dep3) { }
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0010").ShouldBeFalse();
}
