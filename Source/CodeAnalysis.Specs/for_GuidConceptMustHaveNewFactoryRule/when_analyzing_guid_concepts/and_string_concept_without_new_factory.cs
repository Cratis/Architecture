// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_GuidConceptMustHaveNewFactoryRule.when_analyzing_guid_concepts;

public class and_string_concept_without_new_factory : given.a_guidconceptmusthavenwfactoryrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
public record ConceptAs<T>(T Value);

public record AuthorName(string Value) : ConceptAs<string>(Value)
{
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0028").ShouldBeFalse();
}
