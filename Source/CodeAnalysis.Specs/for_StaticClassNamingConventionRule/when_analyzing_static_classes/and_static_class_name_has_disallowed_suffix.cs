// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_StaticClassNamingConventionRule.when_analyzing_static_classes;

public class and_static_class_name_has_disallowed_suffix : given.a_staticclassnamingconventionrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
public static class Utility
{
}
""");

    [Fact] void should_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0015").ShouldBeTrue();
}
