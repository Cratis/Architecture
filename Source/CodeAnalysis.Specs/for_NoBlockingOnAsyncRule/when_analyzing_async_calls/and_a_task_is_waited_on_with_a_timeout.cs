// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_NoBlockingOnAsyncRule.when_analyzing_async_calls;

public class and_a_task_is_waited_on_with_a_timeout : given.a_noblockingonasyncrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
using System.Threading.Tasks;

class Sample
{
    bool Run() => Task.Delay(1).Wait(10);
}
""");

    [Fact]
    void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0013").ShouldBeFalse();
}
