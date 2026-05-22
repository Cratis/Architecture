// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs;

public partial class ArchitectureAnalyzerSpecs
{
    [Fact]
    public async Task ShouldWarnForMismatchedLoggerCategory()
    {
        const string source = """
namespace Microsoft.Extensions.Logging
{
    public interface ILogger<TCategoryName>
    {
    }
}

class OtherType
{
}

class Handler
{
    public Handler(Microsoft.Extensions.Logging.ILogger<OtherType> logger)
    {
    }
}
""";

        var diagnostics = await Analyze(source);

        Assert.Contains(diagnostics, _ => _.Id == "CRARCH0023");
    }

    [Fact]
    public async Task ShouldNotWarnForContainingTypeLoggerCategory()
    {
        const string source = """
namespace Microsoft.Extensions.Logging
{
    public interface ILogger<TCategoryName>
    {
    }
}

class Handler
{
    public Handler(Microsoft.Extensions.Logging.ILogger<Handler> logger)
    {
    }
}
""";

        var diagnostics = await Analyze(source);

        Assert.DoesNotContain(diagnostics, _ => _.Id == "CRARCH0023");
    }
}
