// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_file_length_threshold.when_analyzing_source_code;

public class and_effective_lines_exceed_threshold : given.an_architecture_analyzer
{
    async Task Because()
    {
        var members = string.Join(Environment.NewLine, Enumerable.Range(0, 401).Select(index => $"    int field{index};"));
        var source = $"class Sample{Environment.NewLine}{{{Environment.NewLine}{members}{Environment.NewLine}}}";
        _diagnostics = await analyze(source);
    }

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0011").ShouldBeTrue();
}
