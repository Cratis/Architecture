// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.for_ArchitectureAnalyzer.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_ArchitectureAnalyzer.when_analyzing_source_code;

public class and_loggermessage_container_follows_conventions : given.an_architecture_analyzer
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
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
""");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0024").ShouldBeFalse();
}
