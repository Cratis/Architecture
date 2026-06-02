// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_no_postfixes_on_class_names.when_analyzing_source_code;

public class and_class_name_has_forbidden_postfix : given.an_architecture_analyzer
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
class AuthorService
{
}
""");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0003").ShouldBeTrue();
}
