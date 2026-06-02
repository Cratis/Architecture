// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_no_blocking_on_async.when_analyzing_source_code;

public class and_async_result_is_accessed_synchronously : given.an_architecture_analyzer
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
using System.Threading.Tasks;

class Sample
{
    int GetValue() => Task.FromResult(42).Result;
}
""");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0013").ShouldBeTrue();
}
