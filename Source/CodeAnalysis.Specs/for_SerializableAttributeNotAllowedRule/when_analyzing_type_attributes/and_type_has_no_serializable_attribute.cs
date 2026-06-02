// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_SerializableAttributeNotAllowedRule.when_analyzing_type_attributes;

public class and_type_has_no_serializable_attribute : given.a_serializableattributenotallowedrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
class Sample
{
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0021").ShouldBeFalse();
}
