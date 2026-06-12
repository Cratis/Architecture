// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that forbids technical postfixes such as Async, Impl, Manager, Helper, and Service on class names.
/// </summary>
public static class NoPostfixesOnClassNamesRule
{
    static readonly string[] _classNameSuffixes = ["Async", "Impl", "Manager", "Helper", "Service"];

    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0003";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "No postfixes on class names", "Class '{0}' must not end with postfix '{1}'", "Rename classes to domain concepts and remove technical postfixes such as Async, Impl, Manager, Helper, and Service.");

    /// <summary>
    /// Analyzes a named type symbol and reports a diagnostic if the class name ends with a forbidden technical postfix.
    /// </summary>
    /// <param name="context">The <see cref="SymbolAnalysisContext"/> for the current analysis.</param>
    /// <param name="type">The <see cref="INamedTypeSymbol"/> to analyze.</param>
    public static void Analyze(SymbolAnalysisContext context, INamedTypeSymbol type)
    {
        if (type.TypeKind != TypeKind.Class)
        {
            return;
        }

        foreach (var suffix in _classNameSuffixes)
        {
            if (type.Name.EndsWith(suffix, StringComparison.Ordinal))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, type.Locations.FirstOrDefault(), type.Name, suffix));
                break;
            }
        }
    }
}
