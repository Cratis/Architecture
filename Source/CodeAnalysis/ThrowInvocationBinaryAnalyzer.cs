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
    static void AnalyzeThrow(SyntaxNodeAnalysisContext context)
    {
        ExpressionSyntax? expression = context.Node switch
        {
            ThrowStatementSyntax statement => statement.Expression,
            ThrowExpressionSyntax throwExpression => throwExpression.Expression,
            _ => null,
        };

        if (expression is null)
        {
            return;
        }

        var type = context.SemanticModel.GetTypeInfo(expression, context.CancellationToken).Type;
        var typeName = type?.ToDisplayString();
        if (typeName is not null && _builtInExceptions.Contains(typeName))
        {
            context.ReportDiagnostic(Diagnostic.Create(_rule0002, expression.GetLocation(), type!.Name));
        }
    }

    static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation)
        {
            return;
        }

        if (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is not IMethodSymbol method)
        {
            return;
        }

        if (method.ContainingType?.SpecialType == SpecialType.System_String && method.Name == "Format")
        {
            context.ReportDiagnostic(Diagnostic.Create(_rule0009, invocation.GetLocation()));
        }

        if (method.Name.StartsWith("Log", StringComparison.Ordinal) &&
            (method.ContainingType?.ToDisplayString().Contains("ILogger", StringComparison.Ordinal) == true ||
             method.ContainingNamespace?.ToDisplayString().Contains("Microsoft.Extensions.Logging", StringComparison.Ordinal) == true))
        {
            context.ReportDiagnostic(Diagnostic.Create(_rule0006, invocation.GetLocation()));
        }

        if (method.Name == "Wait" && method.Parameters.Length == 0)
        {
            context.ReportDiagnostic(Diagnostic.Create(_rule0013, invocation.GetLocation()));
        }

        if (method.Name == "GetResult" &&
            method.ContainingType?.Name == "TaskAwaiter" &&
            invocation.Expression is MemberAccessExpressionSyntax { Expression: InvocationExpressionSyntax awaiterInvocation } &&
            context.SemanticModel.GetSymbolInfo(awaiterInvocation, context.CancellationToken).Symbol is IMethodSymbol awaiterMethod &&
            awaiterMethod.Name == "GetAwaiter")
        {
            context.ReportDiagnostic(Diagnostic.Create(_rule0013, invocation.GetLocation()));
        }

        if (method.Name == "StartActivity" &&
            method.ContainingType?.ToDisplayString() == "System.Diagnostics.ActivitySource")
        {
            context.ReportDiagnostic(Diagnostic.Create(_rule0025, invocation.GetLocation()));
        }

        if (method.Name.StartsWith("Create", StringComparison.Ordinal) &&
            method.ContainingType?.ToDisplayString() == "System.Diagnostics.Metrics.Meter")
        {
            context.ReportDiagnostic(Diagnostic.Create(_rule0026, invocation.GetLocation(), method.Name));
        }

        if (ReturnsTaskLike(method.ReturnType) && IsUnhandledAsyncInvocation(invocation, method))
        {
            context.ReportDiagnostic(Diagnostic.Create(_rule0020, invocation.GetLocation(), method.Name));
        }
    }

    static void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not BinaryExpressionSyntax expression)
        {
            return;
        }

        if (expression.IsKind(SyntaxKind.EqualsExpression) || expression.IsKind(SyntaxKind.NotEqualsExpression))
        {
            if (expression.Left.IsKind(SyntaxKind.NullLiteralExpression) || expression.Right.IsKind(SyntaxKind.NullLiteralExpression))
            {
                context.ReportDiagnostic(Diagnostic.Create(_rule0008, expression.GetLocation()));
            }

            return;
        }

        if (expression.IsKind(SyntaxKind.AddExpression))
        {
            var type = context.SemanticModel.GetTypeInfo(expression, context.CancellationToken).Type;
            if (type?.SpecialType == SpecialType.System_String)
            {
                context.ReportDiagnostic(Diagnostic.Create(_rule0009, expression.GetLocation()));
            }
        }
    }

    static bool ReturnsTaskLike(ITypeSymbol returnType)
        => returnType.ToDisplayString() is "System.Threading.Tasks.Task" or "System.Threading.Tasks.ValueTask" ||
           (returnType is INamedTypeSymbol namedType && namedType.IsGenericType &&
            (namedType.ConstructedFrom.ToDisplayString() is "System.Threading.Tasks.Task<T>" or "System.Threading.Tasks.ValueTask<T>"));

    static bool IsUnhandledAsyncInvocation(InvocationExpressionSyntax invocation, IMethodSymbol method)
    {
        if (method.Name == "ContinueWith")
        {
            return false;
        }

        if (invocation.Parent is AwaitExpressionSyntax or ReturnStatementSyntax or ArrowExpressionClauseSyntax)
        {
            return false;
        }

        if (invocation.Parent is MemberAccessExpressionSyntax { Name.Identifier.Text: "ContinueWith" })
        {
            return false;
        }

        return invocation.Parent is ExpressionStatementSyntax;
    }
}
