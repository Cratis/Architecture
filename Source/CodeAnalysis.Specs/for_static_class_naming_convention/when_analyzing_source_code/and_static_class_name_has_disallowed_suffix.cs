// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_static_class_naming_convention.when_analyzing_source_code;

public class and_static_class_name_has_disallowed_suffix : given.an_architecture_analyzer
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
public static class Utility
{
}
""");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0015").ShouldBeTrue();
}
