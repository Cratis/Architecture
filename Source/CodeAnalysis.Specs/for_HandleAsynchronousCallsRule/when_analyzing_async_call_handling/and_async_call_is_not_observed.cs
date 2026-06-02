// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_HandleAsynchronousCallsRule.when_analyzing_async_call_handling;

public class and_async_call_is_not_observed : given.a_handleasynchronouscallsrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
using System.Threading.Tasks;

class Sample
{
    void Fire()
    {
        Task.Delay(1);
    }
}
""");

    [Fact] void should_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0020").ShouldBeTrue();
}
