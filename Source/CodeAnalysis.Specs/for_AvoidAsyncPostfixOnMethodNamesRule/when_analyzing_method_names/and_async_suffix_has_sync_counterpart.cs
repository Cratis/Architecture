// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_AvoidAsyncPostfixOnMethodNamesRule.when_analyzing_method_names;

public class and_async_suffix_has_sync_counterpart : given.a_avoidasyncpostfixonmethodnamesrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
using System.Threading.Tasks;

class Sample
{
    void Handle() { }
    Task HandleAsync() => Task.CompletedTask;
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0019").ShouldBeFalse();
}
