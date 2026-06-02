// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_constructor_fan_out.when_analyzing_source_code;

public class and_constructor_has_more_than_seven_dependencies : given.an_architecture_analyzer
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
class Sample
{
    public Sample(
        int dependency1,
        int dependency2,
        int dependency3,
        int dependency4,
        int dependency5,
        int dependency6,
        int dependency7,
        int dependency8)
    {
    }
}
""");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0010").ShouldBeTrue();
}
