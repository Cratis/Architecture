// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_LoggerMessageContainerConventionsRule.given;

public class a_loggermessagecontainerconventionsrule : Cratis.Architecture.CodeAnalysis.Specs.given.an_architecture_analyzer
{
    protected const string LoggingTypes =
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

""";

    protected static Task<IReadOnlyList<Microsoft.CodeAnalysis.Diagnostic>> analyze_container(string container) =>
        analyze(LoggingTypes + container);
}
