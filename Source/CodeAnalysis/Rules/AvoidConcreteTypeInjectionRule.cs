// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class AvoidConcreteTypeInjectionRule
{
    public const string Id = "CRARCH0018";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Avoid concrete type injection", "Constructor dependency '{0}' should be an interface abstraction", "Inject interfaces instead of concrete classes. Concrete types marked with [ReadModel] are exempt.");
}
