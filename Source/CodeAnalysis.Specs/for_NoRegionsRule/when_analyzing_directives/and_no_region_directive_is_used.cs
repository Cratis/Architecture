// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_NoRegionsRule.when_analyzing_directives;

public class and_no_region_directive_is_used : given.a_noregionsrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
class Sample
{
    void Method() { }
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0005").ShouldBeFalse();
}
