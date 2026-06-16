// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_ConceptAsMustHaveNotSetSentinelRule.when_analyzing_concept_records;

public class and_concept_has_empty_sentinel : given.a_conceptasmusthavenotsetssentinelrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
public record ConceptAs<T>(T Value);

public record AuthorName(string Value) : ConceptAs<string>(Value)
{
    public static readonly AuthorName Empty = new(string.Empty);
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0027").ShouldBeFalse();
}
