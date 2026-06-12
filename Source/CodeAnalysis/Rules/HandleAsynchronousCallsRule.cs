// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that enforces asynchronous calls are awaited or have a continuation rather than being fire-and-forget.
/// </summary>
public static class HandleAsynchronousCallsRule
{
    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0020";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Handle asynchronous calls", "Asynchronous call '{0}' must be handled by awaiting it or chaining a continuation", "Do not fire-and-forget asynchronous calls. Await them or chain a continuation.");

    /// <summary>
    /// Analyzes an invocation expression and reports a diagnostic if the async call result is unhandled.
    /// </summary>
    /// <param name="context">The <see cref="SyntaxNodeAnalysisContext"/> for the current analysis.</param>
    /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/> to analyze.</param>
    /// <param name="method">The <see cref="IMethodSymbol"/> of the invoked method.</param>
    public static void Analyze(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, IMethodSymbol method)
    {
        if (RuleAnalysisUtilities.ReturnsTaskLike(method.ReturnType) && IsUnhandledAsyncInvocation(invocation, method))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, invocation.GetLocation(), method.Name));
        }
    }

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
