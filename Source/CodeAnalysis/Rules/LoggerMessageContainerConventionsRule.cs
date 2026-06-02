// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

public static class LoggerMessageContainerConventionsRule
{
    public const string Id = "CRARCH0024";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "LoggerMessage container conventions", "Type '{0}' with [LoggerMessage] methods must be an internal static partial *LogMessages class", "Place [LoggerMessage] methods in an internal static partial class named with the LogMessages suffix.");

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
