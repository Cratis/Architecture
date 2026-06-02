// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class AvoidAsyncPostfixOnMethodNamesRule
{
    public const string Id = "CRARCH0019";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Avoid Async postfix on method names", "Method '{0}' should not end with 'Async' unless a synchronous '{1}' method also exists", "Rename async methods to omit the Async suffix unless the type also exposes an explicit synchronous method with the same base name.");

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
