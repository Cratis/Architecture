// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class PrivateModifierNotAllowedRule
{
    public const string Id = "CRARCH0022";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Private modifier not allowed", "Avoid explicit 'private' modifier", "Remove explicit private modifiers because private is implicit in C#. Keep explicit private only for property setters.");
}
