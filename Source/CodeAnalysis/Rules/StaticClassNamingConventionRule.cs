// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that enforces static classes use approved name suffixes such as Extensions, Converters, Ids, WellKnown, or Defaults.
/// </summary>
public static class StaticClassNamingConventionRule
{
    static readonly string[] _staticClassNameSuffixes = ["Extensions", "Converters", "Ids", "WellKnown", "Defaults"];

    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0015";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Static class naming convention", "Static class '{0}' must end with one of: {1}", "Rename static classes to one of the approved suffixes: Extensions, Converters, Ids, WellKnown, or Defaults.");

    /// <summary>
    /// Analyzes a named type symbol and reports a diagnostic if a static class does not use an approved name suffix.
    /// </summary>
    /// <param name="context">The <see cref="SymbolAnalysisContext"/> for the current analysis.</param>
    /// <param name="type">The <see cref="INamedTypeSymbol"/> to analyze.</param>
    public static void Analyze(SymbolAnalysisContext context, INamedTypeSymbol type)
    {
        if (type.TypeKind == TypeKind.Class && type.IsStatic && !_staticClassNameSuffixes.Any(type.Name.EndsWith))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, type.Locations.FirstOrDefault(), type.Name, string.Join(", ", _staticClassNameSuffixes)));
        }
    }
}
