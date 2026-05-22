// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs;

public partial class ArchitectureAnalyzerSpecs
{
    [Fact]
    public async Task ShouldWarnForDirectLoggerInvocation()
    {
        const string source = """
namespace Microsoft.Extensions.Logging
{
    public interface ILogger<TCategoryName>
    {
        void LogInformation(string message);
    }
}

class Handler
{
    readonly Microsoft.Extensions.Logging.ILogger<Handler> _logger;

    public Handler(Microsoft.Extensions.Logging.ILogger<Handler> logger)
    {
        _logger = logger;
    }

    public void Execute()
    {
        _logger.LogInformation("Hello");
    }
}
""";

        var diagnostics = await Analyze(source);

        Assert.Contains(diagnostics, _ => _.Id == "CRARCH0006");
    }
}
