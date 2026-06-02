// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_NoTestTypesInProductionRule.when_analyzing_type_references;

public class and_production_code_references_specs_type : given.a_notesttypesinproductionrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
namespace Company.Specs
{
    public class FakeDependency { }
}

class Sample
{
    Company.Specs.FakeDependency _dependency = new();
}
""");

    [Fact] void should_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0014").ShouldBeTrue();
}
