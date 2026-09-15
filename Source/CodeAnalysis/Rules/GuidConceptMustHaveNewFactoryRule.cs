// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that ensures Guid-backed ConceptAs records have a static New() factory method.
/// </summary>
public static class GuidConceptMustHaveNewFactoryRule
{
    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0028";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(
            Id,
            "Guid-backed identity concept must have a static New() factory",
            "Record '{0}' inheriting from ConceptAs<Guid> must have a static method named 'New()' returning the concept type",
            "Add a static factory method for creating new instances: 'public static {0} New() => new(Guid.NewGuid());'. This reads better than 'new {0}(Guid.NewGuid())' and makes the intent of creating a new identity explicit.");

    /// <summary>
    /// Analyzes a named type symbol and reports a diagnostic if it inherits from ConceptAs&lt;Guid&gt; without a New() factory.
    /// </summary>
    /// <param name="context">The <see cref="SymbolAnalysisContext"/> for the current analysis.</param>
    /// <param name="type">The <see cref="INamedTypeSymbol"/> to analyze.</param>
    public static void Analyze(SymbolAnalysisContext context, INamedTypeSymbol type)
    {
        // Only analyze records
        if (type.TypeKind != TypeKind.Class || !type.IsRecord)
        {
            return;
        }

        // Check if this type inherits from ConceptAs<T>
        var baseType = type.BaseType;
        if (baseType is null || baseType.Name != "ConceptAs" || !baseType.IsGenericType)
        {
            return;
        }

        // Check if the type argument is Guid
        var typeArgument = baseType.TypeArguments.FirstOrDefault();
        if (typeArgument is null)
        {
            return;
        }

        // Guid doesn't have a special type, so check by name and namespace
        if (typeArgument.Name != "Guid" || typeArgument.ContainingNamespace?.ToDisplayString() != "System")
        {
            return;
        }

        // Check if the record has a static New() method returning the concept type
        var hasNewFactory = type.GetMembers()
            .OfType<IMethodSymbol>()
            .Any(method =>
                method.IsStatic &&
                method.Name == "New" &&
                method.Parameters.Length == 0 &&
                SymbolEqualityComparer.Default.Equals(method.ReturnType, type));

        if (!hasNewFactory)
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, type.Locations.FirstOrDefault(), type.Name));
        }
    }
}
