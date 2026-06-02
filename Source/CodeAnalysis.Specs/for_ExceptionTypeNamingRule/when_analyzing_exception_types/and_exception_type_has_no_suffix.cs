// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Architecture.CodeAnalysis.Specs.for_ExceptionTypeNamingRule.when_analyzing_exception_types;

public class and_exception_type_has_no_suffix : given.a_exceptiontypenamingrule
{
    async Task Because() =>
        _diagnostics = await analyze(
            """
using System;

class AuthorNotFound : Exception
{
}
""");

    [Fact] void should_not_report_diagnostic() =>
        _diagnostics.Any(_ => _.Id == "CRARCH0001").ShouldBeFalse();
}
