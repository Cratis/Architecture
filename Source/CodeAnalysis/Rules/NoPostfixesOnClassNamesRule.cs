// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class NoPostfixesOnClassNamesRule
{
    public const string Id = "CRARCH0003";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "No postfixes on class names", "Class '{0}' must not end with postfix '{1}'", "Rename classes to domain concepts and remove technical postfixes such as Async, Impl, Manager, Helper, and Service.");
}
