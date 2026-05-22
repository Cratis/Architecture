// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs;

public partial class ArchitectureAnalyzerSpecs
{
    [Fact]
    public async Task ShouldNotWarnWhenLoggerMessageContainerFollowsConventions()
    {
        const string source = """
namespace Microsoft.Extensions.Logging
{
    public enum LogLevel
    {
        Information
    }

    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class LoggerMessageAttribute : System.Attribute
    {
        public LoggerMessageAttribute(LogLevel level, string message)
        {
        }
    }

    public interface ILogger
    {
    }
}

internal static partial class HandlerLogMessages
{
    [Microsoft.Extensions.Logging.LoggerMessage(Microsoft.Extensions.Logging.LogLevel.Information, "Starting handler")]
    static void Starting(this Microsoft.Extensions.Logging.ILogger logger)
    {
    }
}
""";

        var diagnostics = await Analyze(source);

        Assert.DoesNotContain(diagnostics, _ => _.Id == "CRARCH0024");
    }
}
