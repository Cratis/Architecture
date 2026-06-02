// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_namespace_must_align_with_folder_path.when_analyzing_source_code;

public class and_namespace_does_not_match_logical_folder_path : given.an_architecture_analyzer
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
namespace Company.Orders;

class Sample
{
}
""",
            "/tmp/Source/CodeAnalysis/SubFolder/Components/TestFile.cs");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0017").ShouldBeTrue();
}
