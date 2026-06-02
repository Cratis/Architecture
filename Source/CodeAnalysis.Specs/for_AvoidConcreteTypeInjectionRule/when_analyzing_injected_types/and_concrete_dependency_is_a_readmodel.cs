// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_AvoidConcreteTypeInjectionRule.when_analyzing_injected_types;

public class and_concrete_dependency_is_a_readmodel : given.a_avoidconcretetypeinjectionrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
namespace Cratis.Arc.Queries.ModelBound
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class ReadModelAttribute : System.Attribute { }
}

[Cratis.Arc.Queries.ModelBound.ReadModel]
class AuthorReadModel
{
}

class Sample(AuthorReadModel readModel)
{
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0018").ShouldBeFalse();
}
