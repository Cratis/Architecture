// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_NoBlockingOnAsyncRule.when_analyzing_async_calls;

public class and_async_result_is_awaited : given.a_noblockingonasyncrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
using System.Threading.Tasks;

class Sample
{
    async Task<int> GetValue() => await Task.FromResult(42);
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0013").ShouldBeFalse();
}
