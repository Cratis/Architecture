// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that enforces exception type names do not end with the Exception suffix.
/// </summary>
public static class ExceptionTypeNamingRule
{
    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0001";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Exception type naming", "Exception type '{0}' must not end with 'Exception'", "Rename exception types to domain terms without the Exception suffix, for example AuthorNotFound instead of AuthorNotFoundException.");

    /// <summary>
    /// Analyzes a named type symbol and reports a diagnostic if it is an exception class ending with Exception.
    /// </summary>
    /// <param name="context">The <see cref="SymbolAnalysisContext"/> for the current analysis.</param>
    /// <param name="type">The <see cref="INamedTypeSymbol"/> to analyze.</param>
    public static void Analyze(SymbolAnalysisContext context, INamedTypeSymbol type)
    {
        if (type.TypeKind == TypeKind.Class && type.BaseType?.ToDisplayString() == "System.Exception" && type.Name.EndsWith("Exception", StringComparison.Ordinal))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, type.Locations.FirstOrDefault(), type.Name));
        }
    }
}
