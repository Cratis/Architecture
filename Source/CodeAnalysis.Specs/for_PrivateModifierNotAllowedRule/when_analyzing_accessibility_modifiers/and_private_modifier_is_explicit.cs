// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_PrivateModifierNotAllowedRule.when_analyzing_accessibility_modifiers;

public class and_private_modifier_is_explicit : given.a_privatemodifiernotallowedrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
class Sample
{
    private int field;
}
""");

    [Fact] void should_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0022").ShouldBeTrue();
}
