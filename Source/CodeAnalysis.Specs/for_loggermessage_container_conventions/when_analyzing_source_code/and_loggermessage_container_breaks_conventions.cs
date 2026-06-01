// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_loggermessage_container_conventions.when_analyzing_source_code;

public class and_loggermessage_container_breaks_conventions : given.an_architecture_analyzer
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

static class HandlerLogging
{
    [Microsoft.Extensions.Logging.LoggerMessage(Microsoft.Extensions.Logging.LogLevel.Information, "Starting handler")]
    public static void Starting(this Microsoft.Extensions.Logging.ILogger logger)
    {
    }
}
""");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0024").ShouldBeTrue();
}
