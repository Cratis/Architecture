// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_NoPostfixesOnClassNamesRule.when_analyzing_class_names;

public class and_class_name_has_no_postfix : given.a_nopostfixesonclassnamesrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
class Author
{
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0003").ShouldBeFalse();
}
