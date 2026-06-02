// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_LoggerMessageContainerConventionsRule.when_analyzing_loggermessage_containers;

public class and_loggermessage_container_breaks_conventions : given.a_loggermessagecontainerconventionsrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
namespace Microsoft.Extensions.Logging
{
    public sealed class LoggerMessageAttribute : System.Attribute
    {
        public LoggerMessageAttribute(int eventId, LogLevel level, string message) { }
    }
    public enum LogLevel { Information }
    public interface ILogger { }
}

public class LogMessages
{
    [Microsoft.Extensions.Logging.LoggerMessage(0, Microsoft.Extensions.Logging.LogLevel.Information, "message")]
    public static partial void Log(this Microsoft.Extensions.Logging.ILogger logger);
}
""");

    [Fact] void should_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0024").ShouldBeTrue();
}
