// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_private_modifier_not_allowed.when_analyzing_source_code;

public class and_private_modifier_is_explicit : given.an_architecture_analyzer
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
class Handler
{
    private int _count;
}
""");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0022").ShouldBeTrue();
}
