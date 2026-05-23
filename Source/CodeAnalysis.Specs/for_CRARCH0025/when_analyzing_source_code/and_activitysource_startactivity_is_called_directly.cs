// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_CRARCH0025.when_analyzing_source_code;

public class and_activitysource_startactivity_is_called_directly : given.an_architecture_analyzer
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
using System.Diagnostics;

class Handler
{
    readonly ActivitySource _source = new("MyService");

    public void Execute()
    {
        using var scope = _source.StartActivity("process");
    }
}
""");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0025").ShouldBeTrue();
}
