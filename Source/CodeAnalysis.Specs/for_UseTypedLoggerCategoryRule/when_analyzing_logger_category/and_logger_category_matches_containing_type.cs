// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_UseTypedLoggerCategoryRule.when_analyzing_logger_category;

public class and_logger_category_matches_containing_type : given.a_usetypedloggercategoryrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
namespace Microsoft.Extensions.Logging
{
    public interface ILogger<TCategoryName> { }
}

class Handler(Microsoft.Extensions.Logging.ILogger<Handler> logger)
{
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0023").ShouldBeFalse();
}
