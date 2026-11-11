// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_LoggerMessageContainerConventionsRule.when_analyzing_loggermessage_containers;

public class and_only_the_name_suffix_is_missing : given.a_loggermessagecontainerconventionsrule
{
    async Task Because() =>
        _diagnostics = await analyze_container(
            """
internal static partial class SampleLogging
{
    [Microsoft.Extensions.Logging.LoggerMessage(0, Microsoft.Extensions.Logging.LogLevel.Information, "message")]
    public static partial void LogSomething(this Microsoft.Extensions.Logging.ILogger logger);
}
""");

    [Fact]
    void should_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0024").ShouldBeTrue();
}
