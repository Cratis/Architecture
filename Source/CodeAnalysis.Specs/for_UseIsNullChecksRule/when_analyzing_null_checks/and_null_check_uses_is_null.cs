// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_UseIsNullChecksRule.when_analyzing_null_checks;

public class and_null_check_uses_is_null : given.a_useisnullchecksrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
class Sample
{
    bool Check(string value) => value is null;
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0008").ShouldBeFalse();
}
