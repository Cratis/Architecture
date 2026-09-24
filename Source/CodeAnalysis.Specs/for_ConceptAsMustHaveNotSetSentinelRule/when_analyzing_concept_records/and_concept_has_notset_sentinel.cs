// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_ConceptAsMustHaveNotSetSentinelRule.when_analyzing_concept_records;

public class and_concept_has_notset_sentinel : given.a_conceptasmusthavenotsetssentinelrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
using System;

public record ConceptAs<T>(T Value);

public record AuthorId(Guid Value) : ConceptAs<Guid>(Value)
{
    public static readonly AuthorId NotSet = new(Guid.Empty);
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0027").ShouldBeFalse();
}
