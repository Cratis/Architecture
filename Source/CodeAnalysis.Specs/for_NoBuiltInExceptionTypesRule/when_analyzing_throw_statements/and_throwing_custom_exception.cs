// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_NoBuiltInExceptionTypesRule.when_analyzing_throw_statements;

public class and_throwing_custom_exception : given.a_nobuiltinexceptiontypesrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
using System;

class CustomException : Exception { }

class Sample
{
    void Throw() => throw new CustomException();
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0002").ShouldBeFalse();
}
