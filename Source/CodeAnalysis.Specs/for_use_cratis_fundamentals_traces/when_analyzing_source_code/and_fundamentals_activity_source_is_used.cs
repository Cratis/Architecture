// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_use_cratis_fundamentals_traces.when_analyzing_source_code;

public class and_fundamentals_activity_source_is_used : given.an_architecture_analyzer
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
namespace Cratis.Traces
{
    public interface IActivitySource<T>
    {
    }

    public interface IActivityScope<T> : System.IDisposable
    {
    }
}

static class HandlerTraces
{
    public static Cratis.Traces.IActivityScope<Handler> Process(this Cratis.Traces.IActivitySource<Handler> source)
        => default!;
}

class Handler
{
    readonly Cratis.Traces.IActivitySource<Handler> _source;

    public Handler(Cratis.Traces.IActivitySource<Handler> source)
    {
        _source = source;
    }

    public void Execute()
    {
        using var scope = _source.Process();
    }
}
""");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0025").ShouldBeFalse();
}
