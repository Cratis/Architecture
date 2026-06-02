// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class AvoidAsyncPostfixOnMethodNamesRule
{
    public const string Id = "CRARCH0019";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Avoid Async postfix on method names", "Method '{0}' should not end with 'Async' unless a synchronous '{1}' method also exists", "Rename async methods to omit the Async suffix unless the type also exposes an explicit synchronous method with the same base name.");
}
