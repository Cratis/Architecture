// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_CRARCH0018.when_analyzing_source_code;

public class and_concrete_dependency_is_a_readmodel : given.an_architecture_analyzer
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
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
""");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0018").ShouldBeFalse();
}
