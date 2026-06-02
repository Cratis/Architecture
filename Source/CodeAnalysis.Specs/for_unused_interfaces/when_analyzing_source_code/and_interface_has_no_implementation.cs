// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_unused_interfaces.when_analyzing_source_code;

public class and_interface_has_no_implementation : given.an_architecture_analyzer
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
public interface IAuthorRepository
{
}
""");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0016").ShouldBeTrue();
}
