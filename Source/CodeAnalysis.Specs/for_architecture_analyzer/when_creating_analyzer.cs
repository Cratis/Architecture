// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_architecture_analyzer;

public class when_creating_analyzer : Specification
{
    ArchitectureAnalyzer _analyzer = null!;

    void Establish() => _analyzer = new();

    [Fact] void should_have_supported_diagnostics() => _analyzer.SupportedDiagnostics.ShouldNotBeEmpty();

    [Fact]
    void should_support_all_expected_diagnostics()
    {
        var expectedIds = new[]
        {
            "CRARCH0001",
            "CRARCH0002",
            "CRARCH0003",
            "CRARCH0004",
            "CRARCH0005",
            "CRARCH0006",
            "CRARCH0007",
            "CRARCH0008",
            "CRARCH0009",
            "CRARCH0010",
            "CRARCH0011",
            "CRARCH0012",
            "CRARCH0013",
            "CRARCH0014",
            "CRARCH0015",
            "CRARCH0016",
            "CRARCH0017",
            "CRARCH0018",
            "CRARCH0019",
            "CRARCH0020",
            "CRARCH0021",
            "CRARCH0022",
            "CRARCH0023",
            "CRARCH0024",
            "CRARCH0025",
            "CRARCH0026",
        };

        var actualIds = _analyzer.SupportedDiagnostics.Select(_ => _.Id).ToArray();
        Assert.Equal(expectedIds.Length, actualIds.Length);
        foreach (var expectedId in expectedIds)
        {
            Assert.Contains(expectedId, actualIds);
        }
    }
}
