// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_LoggerMessageContainerConventionsRule.when_analyzing_loggermessage_containers;

public class and_the_type_has_no_loggermessage_methods : given.a_loggermessagecontainerconventionsrule
{
    async Task Because() =>
        _diagnostics = await analyze_container(
            """
public class Sample
{
    public void DoSomething() { }
}
""");

    [Fact]
    void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0024").ShouldBeFalse();
}
