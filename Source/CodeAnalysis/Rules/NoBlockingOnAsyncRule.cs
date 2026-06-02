// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

public static class NoBlockingOnAsyncRule
{
    public const string Id = "CRARCH0013";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "No blocking on async", "Avoid blocking async calls via .Result, .Wait(), or .GetAwaiter().GetResult()", "Replace synchronous blocking on asynchronous operations with await to avoid deadlocks and hidden failures.");

    public static void AnalyzeInvocation(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, IMethodSymbol method)
    {
        if (method.Name == "Wait" && method.Parameters.Length == 0)
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, invocation.GetLocation()));
        }

        if (method.Name == "GetResult" &&
            method.ContainingType?.Name == "TaskAwaiter" &&
            invocation.Expression is MemberAccessExpressionSyntax { Expression: InvocationExpressionSyntax awaiterInvocation } &&
            context.SemanticModel.GetSymbolInfo(awaiterInvocation, context.CancellationToken).Symbol is IMethodSymbol awaiterMethod &&
            awaiterMethod.Name == "GetAwaiter")
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, invocation.GetLocation()));
        }
    }

    public static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax memberAccess)
    {
        if (memberAccess.Name.Identifier.Text != "Result")
        {
            return;
        }

        var expressionType = context.SemanticModel.GetTypeInfo(memberAccess.Expression, context.CancellationToken).Type;
        if (expressionType?.ToDisplayString().StartsWith("System.Threading.Tasks.Task", StringComparison.Ordinal) == true)
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, memberAccess.GetLocation()));
        }
    }
}
