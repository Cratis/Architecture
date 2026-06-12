// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that forbids the use of the Serializable attribute on types.
/// </summary>
public static class SerializableAttributeNotAllowedRule
{
    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0021";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Serializable attribute not allowed", "Type '{0}' must not be marked with [Serializable]", "Remove [Serializable] from types. The attribute is legacy for binary serialization and AppDomain scenarios and should not be used in Cratis code.");

    /// <summary>
    /// Analyzes a named type symbol and reports a diagnostic if it is marked with the Serializable attribute.
    /// </summary>
    /// <param name="context">The <see cref="SymbolAnalysisContext"/> for the current analysis.</param>
    /// <param name="type">The <see cref="INamedTypeSymbol"/> to analyze.</param>
    public static void Analyze(SymbolAnalysisContext context, INamedTypeSymbol type)
    {
        if (type.GetAttributes().Any(_ => _.AttributeClass?.ToDisplayString() == "System.SerializableAttribute"))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, type.Locations.FirstOrDefault(), type.Name));
        }
    }
}
