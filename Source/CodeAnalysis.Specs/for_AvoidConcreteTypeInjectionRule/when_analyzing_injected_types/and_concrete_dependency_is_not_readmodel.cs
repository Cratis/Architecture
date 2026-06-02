// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_AvoidConcreteTypeInjectionRule.when_analyzing_injected_types;

public class and_concrete_dependency_is_not_readmodel : given.a_avoidconcretetypeinjectionrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
class ConcreteDependency
{
}

class Sample(ConcreteDependency dependency)
{
}
""");

    [Fact] void should_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0018").ShouldBeTrue();
}
