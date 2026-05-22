// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs;

public partial class ArchitectureAnalyzerSpecs
{
    [Fact]
    public async Task ShouldNotWarnForConcreteInjectionWhenTypeIsMarkedAsReadModel()
    {
        const string source = """
using System;

[AttributeUsage(AttributeTargets.Class)]
class ReadModelAttribute : Attribute
{
}

[ReadModel]
class CustomerReadModel
{
}

class Handler
{
    public Handler(CustomerReadModel customer)
    {
    }
}
""";

        var diagnostics = await Analyze(source);

        Assert.DoesNotContain(diagnostics, _ => _.Id == "CRARCH0018");
    }
}
