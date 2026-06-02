// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_LoggingViaLoggerMessageRule.when_analyzing_logger_usage;

public class and_logger_is_invoked_directly : given.a_loggingvialoggermessagerule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
namespace Microsoft.Extensions.Logging
{
    public interface ILogger
    {
        void LogInformation(string message);
    }
}

class Sample(Microsoft.Extensions.Logging.ILogger logger)
{
    void Log() => logger.LogInformation("message");
}
""");

    [Fact] void should_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0006").ShouldBeTrue();
}
