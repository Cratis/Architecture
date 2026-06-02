// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_UseStringInterpolationRule.when_analyzing_string_formatting;

public class and_string_interpolation_is_used : given.a_usestringinterpolationrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
class Sample
{
    string Build(int count) => $"Count: {count}";
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0009").ShouldBeFalse();
}
