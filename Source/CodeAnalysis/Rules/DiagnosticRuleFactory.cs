// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class DiagnosticRuleFactory
{
    public static DiagnosticDescriptor Create(string id, string title, string messageFormat, string description, DiagnosticSeverity severity = DiagnosticSeverity.Warning)
        => new(id, title, messageFormat, "Architecture", severity, isEnabledByDefault: true, description: description);
}
