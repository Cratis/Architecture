// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_NoIServiceProviderInjectionRule.when_analyzing_constructor_parameters;

public class and_iserviceprovider_is_injected : given.a_noiserviceproviderinjectionrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
namespace System
{
    public interface IServiceProvider { }
}

class Sample(System.IServiceProvider serviceProvider)
{
}
""");

    [Fact] void should_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0007").ShouldBeTrue();
}
