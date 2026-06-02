// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class NoTestTypesInProductionRule
{
    public const string Id = "CRARCH0014";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "No test types in production", "Production code must not reference testing/specification types: '{0}'", "Remove references to .Specs/.Testing types from production code and replace them with production abstractions.", DiagnosticSeverity.Error);
}
