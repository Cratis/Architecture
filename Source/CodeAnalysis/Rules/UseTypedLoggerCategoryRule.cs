// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that enforces typed ILogger categories match the containing type.
/// </summary>
public static class UseTypedLoggerCategoryRule
{
    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0023";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Use typed logger category", "Inject ILogger<{0}> instead of '{1}'", "Use ILogger<TContainingType> for constructor injection so log categories map to the concrete type producing the log.");

    /// <summary>
    /// Analyzes a constructor parameter and reports a diagnostic if it injects a non-generic ILogger or an ILogger with the wrong category type.
    /// </summary>
    /// <param name="context">The <see cref="SymbolAnalysisContext"/> for the current analysis.</param>
    /// <param name="containingType">The <see cref="INamedTypeSymbol"/> of the type that owns the constructor.</param>
    /// <param name="parameter">The <see cref="IParameterSymbol"/> to analyze.</param>
    public static void Analyze(SymbolAnalysisContext context, INamedTypeSymbol containingType, IParameterSymbol parameter)
    {
        if (IsNonGenericLogger(parameter.Type))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, parameter.Locations.FirstOrDefault(), containingType.Name, "ILogger"));
            return;
        }

        if (TryGetLoggerCategory(parameter.Type, out var categoryType) &&
            categoryType is not null &&
            !SymbolEqualityComparer.Default.Equals(categoryType, containingType))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, parameter.Locations.FirstOrDefault(), containingType.Name, parameter.Type.ToDisplayString()));
        }
    }

    static bool IsNonGenericLogger(ITypeSymbol type)
        => type.ToDisplayString() == "Microsoft.Extensions.Logging.ILogger";

    static bool TryGetLoggerCategory(ITypeSymbol type, out ITypeSymbol? categoryType)
    {
        categoryType = null;

        if (type is not INamedTypeSymbol { IsGenericType: true } namedType)
        {
            return false;
        }

        if (namedType.ConstructedFrom.ToDisplayString() != "Microsoft.Extensions.Logging.ILogger<TCategoryName>")
        {
            return false;
        }

        categoryType = namedType.TypeArguments[0];
        return true;
    }
}
