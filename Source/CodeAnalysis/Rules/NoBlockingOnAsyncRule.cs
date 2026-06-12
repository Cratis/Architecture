// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that forbids synchronous blocking on async operations via .Result, .Wait(), or .GetAwaiter().GetResult().
/// </summary>
public static class NoBlockingOnAsyncRule
{
    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0013";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "No blocking on async", "Avoid blocking async calls via .Result, .Wait(), or .GetAwaiter().GetResult()", "Replace synchronous blocking on asynchronous operations with await to avoid deadlocks and hidden failures.");

    /// <summary>
    /// Analyzes an invocation expression and reports a diagnostic if it is a blocking Wait() or GetAwaiter().GetResult() call.
    /// </summary>
    /// <param name="context">The <see cref="SyntaxNodeAnalysisContext"/> for the current analysis.</param>
    /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/> to analyze.</param>
    /// <param name="method">The <see cref="IMethodSymbol"/> of the invoked method.</param>
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

    /// <summary>
    /// Analyzes a member access expression and reports a diagnostic if it accesses the Result property on a Task.
    /// </summary>
    /// <param name="context">The <see cref="SyntaxNodeAnalysisContext"/> for the current analysis.</param>
    /// <param name="memberAccess">The <see cref="MemberAccessExpressionSyntax"/> to analyze.</param>
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
