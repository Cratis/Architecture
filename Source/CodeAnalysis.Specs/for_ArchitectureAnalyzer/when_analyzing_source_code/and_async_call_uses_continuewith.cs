// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.for_ArchitectureAnalyzer.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_ArchitectureAnalyzer.when_analyzing_source_code;

public class and_async_call_uses_continuewith : given.an_architecture_analyzer
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
using System;
using System.Threading.Tasks;

class Handler
{
    Task ProcessAsync() => Task.CompletedTask;

    void Execute()
    {
        ProcessAsync().ContinueWith(_ => Console.WriteLine("done"));
    }
}
""");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0020").ShouldBeFalse();
}
