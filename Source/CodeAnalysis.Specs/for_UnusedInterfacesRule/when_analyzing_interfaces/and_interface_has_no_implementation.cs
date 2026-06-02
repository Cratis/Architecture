// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_UnusedInterfacesRule.when_analyzing_interfaces;

public class and_interface_has_no_implementation : given.a_unusedinterfacesrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
public interface IAuthorRepository
{
}
""");

    [Fact] void should_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0016").ShouldBeTrue();
}
