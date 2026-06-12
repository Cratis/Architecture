// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that enforces constructor dependencies use interface abstractions rather than concrete types.
/// </summary>
public static class AvoidConcreteTypeInjectionRule
{
    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0018";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Avoid concrete type injection", "Constructor dependency '{0}' should be an interface abstraction", "Inject interfaces instead of concrete classes. Concrete types marked with [ReadModel] are exempt.");

    /// <summary>
    /// Analyzes a constructor parameter and reports a diagnostic if it is a concrete type.
    /// </summary>
    /// <param name="context">The <see cref="SymbolAnalysisContext"/> for the current analysis.</param>
    /// <param name="parameter">The <see cref="IParameterSymbol"/> to analyze.</param>
    public static void Analyze(SymbolAnalysisContext context, IParameterSymbol parameter)
    {
        if (ShouldWarnConcreteInjection(parameter.Type))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, parameter.Locations.FirstOrDefault(), parameter.Name));
        }
    }

    static bool ShouldWarnConcreteInjection(ITypeSymbol type)
    {
        if (type is ITypeParameterSymbol)
        {
            return false;
        }

        if (type.TypeKind is TypeKind.Interface or TypeKind.Enum or TypeKind.Struct or TypeKind.Delegate)
        {
            return false;
        }

        if (type.SpecialType != SpecialType.None)
        {
            return false;
        }

        if (type is not INamedTypeSymbol namedType)
        {
            return false;
        }

        if (namedType.Name.EndsWith("Options", StringComparison.Ordinal) || namedType.Name.EndsWith("Settings", StringComparison.Ordinal))
        {
            return false;
        }

        if (namedType.GetAttributes().Any(IsCratisArcReadModelAttribute))
        {
            return false;
        }

        if (namedType.ContainingNamespace?.ToDisplayString().StartsWith("System", StringComparison.Ordinal) == true && namedType.IsSealed)
        {
            return false;
        }

        return namedType.TypeKind == TypeKind.Class && !namedType.IsAbstract;
    }

    static bool IsCratisArcReadModelAttribute(AttributeData attribute)
        => attribute.AttributeClass?.ToDisplayString() == "Cratis.Arc.Queries.ModelBound.ReadModelAttribute";
}
