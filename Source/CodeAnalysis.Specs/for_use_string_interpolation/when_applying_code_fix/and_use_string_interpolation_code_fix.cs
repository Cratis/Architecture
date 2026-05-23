// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_use_string_interpolation.when_applying_code_fix;

public class and_use_string_interpolation_code_fix : given.an_architecture_analyzer
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

    [Fact] void should_apply_the_use_string_interpolation_code_fix() =>
        _fixedSource.Contains("$\"Count: {count}\"", StringComparison.Ordinal).ShouldBeTrue();
}
