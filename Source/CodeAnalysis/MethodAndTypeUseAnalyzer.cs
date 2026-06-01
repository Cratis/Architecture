// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis;

/// <summary>
/// Analyzer enforcing Cratis architecture diagnostics.
/// </summary>
public partial class ArchitectureAnalyzer
{
    static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not MethodDeclarationSyntax method)
        {
            return;
        }

        if (method.Modifiers.Any(SyntaxKind.AsyncKeyword) &&
            method.ReturnType is PredefinedTypeSyntax { Keyword.RawKind: (int)SyntaxKind.VoidKeyword } &&
            !IsTestCode(context.Node.SyntaxTree.FilePath) &&
            !IsEventHandler(context, method))
        {
            context.ReportDiagnostic(Diagnostic.Create(_rule0012, method.Identifier.GetLocation()));
        }

        if (!IsTestCode(context.Node.SyntaxTree.FilePath) &&
            method.Identifier.ValueText.EndsWith("Async", StringComparison.Ordinal) &&
            context.SemanticModel.GetDeclaredSymbol(method, context.CancellationToken) is IMethodSymbol methodSymbol)
        {
            var baseName = methodSymbol.Name[..^"Async".Length];
            if (baseName.Length == 0 || !HasSynchronousCounterpart(methodSymbol.ContainingType, baseName))
            {
                context.ReportDiagnostic(Diagnostic.Create(_rule0019, method.Identifier.GetLocation(), methodSymbol.Name, baseName));
            }
        }
    }

    static bool HasSynchronousCounterpart(INamedTypeSymbol containingType, string baseName)
        => containingType.GetMembers(baseName).OfType<IMethodSymbol>().Any(_ => _.MethodKind == MethodKind.Ordinary && !ReturnsTaskLike(_.ReturnType));

    static bool IsEventHandler(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method)
    {
        if (method.ParameterList.Parameters.Count != 2)
        {
            return false;
        }

        var firstParameterType = method.ParameterList.Parameters[0].Type;
        var secondParameterType = method.ParameterList.Parameters[1].Type;
        if (firstParameterType is null || secondParameterType is null)
        {
            return false;
        }

        var firstType = context.SemanticModel.GetTypeInfo(firstParameterType, context.CancellationToken).Type;
        var secondType = context.SemanticModel.GetTypeInfo(secondParameterType, context.CancellationToken).Type as INamedTypeSymbol;

        if (firstType?.SpecialType != SpecialType.System_Object || secondType is null)
        {
            return false;
        }

        while (secondType is not null)
        {
            if (secondType.ToDisplayString() == "System.EventArgs")
            {
                return true;
            }

            secondType = secondType.BaseType;
        }

        return false;
    }

    static void AnalyzeIdentifierTypeUse(SyntaxNodeAnalysisContext context)
    {
        if (IsTestCode(context.Node.SyntaxTree.FilePath))
        {
            return;
        }

        if (context.SemanticModel.GetSymbolInfo(context.Node, context.CancellationToken).Symbol is not ITypeSymbol symbol)
        {
            return;
        }

        var namespaceName = symbol.ContainingNamespace?.ToDisplayString() ?? string.Empty;
        if (namespaceName.Contains(".Specs", StringComparison.Ordinal) || namespaceName.Contains(".Testing", StringComparison.Ordinal))
        {
            context.ReportDiagnostic(Diagnostic.Create(_rule0014, context.Node.GetLocation(), symbol.ToDisplayString()));
        }
    }

    static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not MemberAccessExpressionSyntax { Name.Identifier.Text: "Result" } memberAccess)
        {
            return;
        }

        var expressionType = context.SemanticModel.GetTypeInfo(memberAccess.Expression, context.CancellationToken).Type;
        if (expressionType?.ToDisplayString().StartsWith("System.Threading.Tasks.Task", StringComparison.Ordinal) == true)
        {
            context.ReportDiagnostic(Diagnostic.Create(_rule0013, memberAccess.GetLocation()));
        }
    }

    static bool IsTestCode(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return false;
        }

        return filePath.Contains(".Specs/", StringComparison.Ordinal) ||
               filePath.Contains(".Specs\\", StringComparison.Ordinal) ||
               filePath.Contains(".Tests/", StringComparison.Ordinal) ||
               filePath.Contains(".Tests\\", StringComparison.Ordinal);
    }
}
