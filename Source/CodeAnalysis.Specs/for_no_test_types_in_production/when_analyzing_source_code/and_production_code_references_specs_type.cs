// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_no_test_types_in_production.when_analyzing_source_code;

public class and_production_code_references_specs_type : given.an_architecture_analyzer
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
namespace Company.Specs
{
    public class FakeDependency
    {
    }
}

class Sample
{
    Company.Specs.FakeDependency _dependency = new();
}
""");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0014").ShouldBeTrue();
}
