// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

public static class SerializableAttributeNotAllowedRule
{
    public const string Id = "CRARCH0021";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Serializable attribute not allowed", "Type '{0}' must not be marked with [Serializable]", "Remove [Serializable] from types. The attribute is legacy for binary serialization and AppDomain scenarios and should not be used in Cratis code.");

    public static void Analyze(SymbolAnalysisContext context, INamedTypeSymbol type)
    {
        if (type.GetAttributes().Any(_ => _.AttributeClass?.ToDisplayString() == "System.SerializableAttribute"))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, type.Locations.FirstOrDefault(), type.Name));
        }
    }
}
