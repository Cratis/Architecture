// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_FileLengthThresholdRule.when_analyzing_file_length;

public class and_effective_lines_are_within_threshold : given.a_filelengththresholdrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
class Sample
{
    int field1;
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0011").ShouldBeFalse();
}
