// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class HandleAsynchronousCallsRule
{
    public const string Id = "CRARCH0020";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Handle asynchronous calls", "Asynchronous call '{0}' must be handled by awaiting it or chaining a continuation", "Do not fire-and-forget asynchronous calls. Await them or chain a continuation.");

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
