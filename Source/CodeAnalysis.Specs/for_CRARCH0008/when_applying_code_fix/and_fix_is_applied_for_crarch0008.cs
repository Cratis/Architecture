// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using given = Cratis.Architecture.CodeAnalysis.Specs.given;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_CRARCH0008.when_applying_code_fix;

public class and_fix_is_applied_for_crarch0008 : given.an_architecture_analyzer
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

    [Fact] void should_apply_the_CRARCH0008_fix() =>
        _fixedSource.Contains("value is null", StringComparison.Ordinal).ShouldBeTrue();
}
