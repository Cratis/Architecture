// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that enforces methods do not carry an Async postfix unless a synchronous counterpart exists.
/// </summary>
public static class AvoidAsyncPostfixOnMethodNamesRule
{
    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0019";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Avoid Async postfix on method names", "Method '{0}' should not end with 'Async' unless a synchronous '{1}' method also exists", "Rename async methods to omit the Async suffix unless the type also exposes an explicit synchronous method with the same base name.");

    /// <summary>
    /// Analyzes a method declaration and reports a diagnostic if the name ends with Async without a synchronous counterpart.
    /// </summary>
    /// <param name="context">The <see cref="SyntaxNodeAnalysisContext"/> for the current analysis.</param>
    /// <param name="method">The <see cref="MethodDeclarationSyntax"/> to analyze.</param>
    public static void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method)
    {
        if (RuleAnalysisUtilities.IsTestCode(context.Node.SyntaxTree.FilePath) ||
            !method.Identifier.ValueText.EndsWith("Async", StringComparison.Ordinal) ||
            context.SemanticModel.GetDeclaredSymbol(method, context.CancellationToken) is not IMethodSymbol methodSymbol)
        {
            return;
        }

        var baseName = methodSymbol.Name[..^"Async".Length];
        if (baseName.Length == 0 || !HasSynchronousCounterpart(methodSymbol.ContainingType, baseName))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, method.Identifier.GetLocation(), methodSymbol.Name, baseName));
        }
    }

    static bool HasSynchronousCounterpart(INamedTypeSymbol containingType, string baseName)
        => containingType.GetMembers(baseName)
                         .OfType<IMethodSymbol>()
                         .Any(_ => _.MethodKind == MethodKind.Ordinary && !RuleAnalysisUtilities.ReturnsTaskLike(_.ReturnType));
}
