// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_AsyncVoidForbiddenRule.when_analyzing_async_methods;

public class and_async_task_method_is_used : given.a_asyncvoidforbiddenrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
using System.Threading.Tasks;

class Sample
{
    async Task Handle()
    {
        await Task.Delay(1);
    }
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0012").ShouldBeFalse();
}
