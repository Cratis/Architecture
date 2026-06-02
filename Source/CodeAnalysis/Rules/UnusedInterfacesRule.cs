// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class UnusedInterfacesRule
{
    public const string Id = "CRARCH0016";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Unused interfaces", "Interface '{0}' has no concrete implementations", "Remove speculative interfaces with no implementations or add a concrete implementation where the abstraction is used.");
}
