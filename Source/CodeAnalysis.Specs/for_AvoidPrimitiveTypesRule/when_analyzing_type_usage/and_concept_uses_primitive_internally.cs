// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_AvoidPrimitiveTypesRule.when_analyzing_type_usage;

public class and_concept_uses_primitive_internally : given.an_avoidprimitivetypesrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
public record ConceptAs<T>(T Value);

public record AuthorId(Guid Value) : ConceptAs<Guid>(Value)
{
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0029").ShouldBeFalse();
}
