// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_async_void_forbidden.when_analyzing_source_code;

public class and_async_void_method_is_not_event_handler : given.an_architecture_analyzer
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
using System.Threading.Tasks;

class Sample
{
    async void Handle()
    {
        await Task.Delay(1);
    }
}
""");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0012").ShouldBeTrue();
}
