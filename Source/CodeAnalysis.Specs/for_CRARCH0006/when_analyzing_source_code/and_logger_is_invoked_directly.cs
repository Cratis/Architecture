// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_CRARCH0006.when_analyzing_source_code;

public class and_logger_is_invoked_directly : given.an_architecture_analyzer
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
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
""");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0006").ShouldBeTrue();
}
