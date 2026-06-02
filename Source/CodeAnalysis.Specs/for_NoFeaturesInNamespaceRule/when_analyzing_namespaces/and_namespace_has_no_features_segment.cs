// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_NoFeaturesInNamespaceRule.when_analyzing_namespaces;

public class and_namespace_has_no_features_segment : given.a_nofeaturesinnamespacerule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
namespace Company.Orders;

class Handler
{
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0004").ShouldBeFalse();
}
