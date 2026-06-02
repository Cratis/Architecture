// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_LoggingViaLoggerMessageRule.when_analyzing_logger_usage;

public class and_logger_message_is_used : given.a_loggingvialoggermessagerule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
namespace Microsoft.Extensions.Logging
{
    public sealed class LoggerMessageAttribute : System.Attribute
    {
        public LoggerMessageAttribute(int eventId, Microsoft.Extensions.Logging.LogLevel level, string message) { }
    }
    public enum LogLevel { Information }
    public interface ILogger { }
}

static partial class Log
{
    [Microsoft.Extensions.Logging.LoggerMessage(0, Microsoft.Extensions.Logging.LogLevel.Information, "message")]
    public static partial void LogSomething(this Microsoft.Extensions.Logging.ILogger logger);
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0006").ShouldBeFalse();
}
