// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.for_ArchitectureAnalyzer.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_ArchitectureAnalyzer.when_analyzing_source_code;

public class and_logger_category_matches_containing_type : given.an_architecture_analyzer
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
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
""");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0023").ShouldBeFalse();
}
