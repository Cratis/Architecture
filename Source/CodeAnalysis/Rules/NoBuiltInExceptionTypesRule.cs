// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class NoBuiltInExceptionTypesRule
{
    public const string Id = "CRARCH0002";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "No built-in exception types", "Throw custom domain exceptions instead of '{0}'", "Replace thrown framework exceptions with domain-specific exception types that express business intent.");
}
