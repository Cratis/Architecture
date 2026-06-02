// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Architecture.CodeAnalysis.Rules;

namespace Cratis.Architecture.CodeAnalysis.Specs.for_NoFeaturesInNamespaceRule;

public class when_creating_analyzer : Specification
{
    [Fact] void should_have_diagnostic_descriptor() => NoFeaturesInNamespaceRule.Descriptor.ShouldNotBeNull();
    [Fact] void should_have_correct_diagnostic_id() => NoFeaturesInNamespaceRule.Descriptor.Id.ShouldEqual("CRARCH0004");
}
