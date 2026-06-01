// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_avoid_concrete_type_injection.when_analyzing_source_code;

public class and_concrete_dependency_has_a_non_cratis_readmodel_attribute : given.an_architecture_analyzer
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
namespace Some.Other.Namespace
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    class ReadModelAttribute : Attribute
    {
    }
}

namespace Sample
{
    using Some.Other.Namespace;

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
}
""");

    [Fact] void should_match_expected_diagnostic_result() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0018").ShouldBeTrue();
}
