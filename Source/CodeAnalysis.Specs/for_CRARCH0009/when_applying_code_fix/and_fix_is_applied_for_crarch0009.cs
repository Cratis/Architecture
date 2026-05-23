// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_CRARCH0009.when_applying_code_fix;

public class and_fix_is_applied_for_crarch0009 : given.an_architecture_analyzer
{
    string _fixedSource = string.Empty;

    async Task Because() =>
        _fixedSource = await apply_code_fix(
            """
using System;

class Sample
{
    string Build(int count) => string.Format("Count: {0}", count);
}
""",
            "CRARCH0009");

    [Fact] void should_apply_the_CRARCH0009_fix() =>
        _fixedSource.Contains("$\"Count: {count}\"", StringComparison.Ordinal).ShouldBeTrue();
}
