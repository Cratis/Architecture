// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_AvoidPrimitiveTypesRule.when_analyzing_type_usage;

public class and_regular_class_has_primitive_property : given.an_avoidprimitivetypesrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
public class SomeUtilityClass
{
    public string Name { get; set; }
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0029").ShouldBeFalse();
}
