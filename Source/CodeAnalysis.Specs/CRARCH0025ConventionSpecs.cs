// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs;

public partial class ArchitectureAnalyzerSpecs
{
    [Fact]
    public async Task ShouldNotWarnWhenUsingFundamentalsActivitySourceAbstractions()
    {
        const string source = """
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
""";

        var diagnostics = await Analyze(source);

        Assert.DoesNotContain(diagnostics, _ => _.Id == "CRARCH0025");
    }
}
