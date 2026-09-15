// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that ensures ConceptAs records have a static readonly NotSet or Empty sentinel field.
/// </summary>
public static class ConceptAsMustHaveNotSetSentinelRule
{
    static readonly string[] _sentinelNames = ["NotSet", "Empty", "Null", "None"];

    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0027";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(
            Id,
            "ConceptAs must have a static readonly NotSet sentinel",
            "Record '{0}' inheriting from ConceptAs<{1}> must have a static readonly field named 'NotSet', 'Empty', 'Null', or 'None' representing a sentinel value",
            "Add a static readonly sentinel field to the ConceptAs record. For Guid-backed concepts use 'public static readonly {0} NotSet = new(Guid.Empty);', for string-backed use 'public static readonly {0} NotSet = new(string.Empty);', and for numeric types use 'public static readonly {0} NotSet = new(0);'. This makes 'no value' explicit and avoids nullable reference type noise.");

    /// <summary>
    /// Analyzes a named type symbol and reports a diagnostic if it inherits from ConceptAs without a NotSet sentinel.
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

        // Check if the record has a static readonly field with an acceptable sentinel name
        var hasNotSetSentinel = type.GetMembers()
            .OfType<IFieldSymbol>()
            .Any(field =>
                field.IsStatic &&
                field.IsReadOnly &&
                _sentinelNames.Contains(field.Name, StringComparer.Ordinal));

        if (!hasNotSetSentinel)
        {
            var typeArgument = baseType.TypeArguments.FirstOrDefault();
            var typeArgumentName = typeArgument?.Name ?? "T";
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, type.Locations.FirstOrDefault(), type.Name, typeArgumentName));
        }
    }
}
