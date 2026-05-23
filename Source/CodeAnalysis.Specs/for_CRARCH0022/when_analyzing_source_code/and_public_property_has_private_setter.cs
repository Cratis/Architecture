// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_CRARCH0022.when_analyzing_source_code;

public class and_public_property_has_private_setter : given.an_architecture_analyzer
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
public class Handler
{
    public int Count { get; private set; }
}
""");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0022").ShouldBeFalse();
}
