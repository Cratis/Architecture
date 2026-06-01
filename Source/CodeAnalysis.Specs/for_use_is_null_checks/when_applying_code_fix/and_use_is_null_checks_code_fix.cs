// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_use_is_null_checks.when_applying_code_fix;

public class and_use_is_null_checks_code_fix : given.an_architecture_analyzer
{
    string _fixedSource = string.Empty;

    async Task Because() =>
        _fixedSource = await apply_code_fix(
            """
class Sample
{
    bool Check(object value) => value == null;
}
""",
            "CRARCH0008");

    [Fact] void should_apply_the_use_is_null_checks_code_fix() =>
        _fixedSource.Contains("value is null", StringComparison.Ordinal).ShouldBeTrue();
}
