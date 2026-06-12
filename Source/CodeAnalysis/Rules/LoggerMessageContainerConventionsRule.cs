// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that enforces types containing LoggerMessage methods follow the internal static partial LogMessages naming convention.
/// </summary>
public static class LoggerMessageContainerConventionsRule
{
    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0024";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "LoggerMessage container conventions", "Type '{0}' with [LoggerMessage] methods must be an internal static partial *LogMessages class", "Place [LoggerMessage] methods in an internal static partial class named with the LogMessages suffix.");

    /// <summary>
    /// Analyzes a named type and reports a diagnostic if it contains LoggerMessage methods but does not follow the container conventions.
    /// </summary>
    /// <param name="context">The <see cref="SymbolAnalysisContext"/> for the current analysis.</param>
    /// <param name="type">The <see cref="INamedTypeSymbol"/> to analyze.</param>
    public static void Analyze(SymbolAnalysisContext context, INamedTypeSymbol type)
    {
        if (HasLoggerMessageMethods(type) &&
            (!type.Name.EndsWith("LogMessages", StringComparison.Ordinal) ||
             !type.IsStatic ||
             type.DeclaredAccessibility != Accessibility.Internal ||
             !IsPartialType(type)))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, type.Locations.FirstOrDefault(), type.Name));
        }
    }

    static bool HasLoggerMessageMethods(INamedTypeSymbol type)
        => type.GetMembers()
               .OfType<IMethodSymbol>()
               .Any(_ => _.GetAttributes().Any(IsLoggerMessageAttribute));

    static bool IsLoggerMessageAttribute(AttributeData attribute)
        => attribute.AttributeClass?.ToDisplayString() == "Microsoft.Extensions.Logging.LoggerMessageAttribute" ||
           attribute.AttributeClass?.Name is "LoggerMessageAttribute" or "LoggerMessage";

    static bool IsPartialType(INamedTypeSymbol type)
        => type.DeclaringSyntaxReferences
               .Select(_ => _.GetSyntax())
               .OfType<TypeDeclarationSyntax>()
               .All(_ => _.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)));
}
