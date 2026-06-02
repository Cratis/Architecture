// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_no_regions.when_analyzing_source_code;

public class and_region_directive_is_used : given.an_architecture_analyzer
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
class Sample
{
#region Region
    void Method()
    {
    }
#endregion
}
""");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0005").ShouldBeTrue();
}
