// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that enforces string interpolation over string.Format calls and string concatenation.
/// </summary>
public static class UseStringInterpolationRule
{
    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0009";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Use string interpolation", "Use string interpolation instead of string.Format or concatenation", "Replace string.Format and string concatenation with interpolated strings ($\"...\") for readability.");

    /// <summary>
    /// Analyzes an invocation expression and reports a diagnostic if it is a string.Format call.
    /// </summary>
    /// <param name="context">The <see cref="SyntaxNodeAnalysisContext"/> for the current analysis.</param>
    /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/> to analyze.</param>
    /// <param name="method">The <see cref="IMethodSymbol"/> of the invoked method.</param>
    public static void AnalyzeInvocation(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, IMethodSymbol method)
    {
        if (method.ContainingType?.SpecialType == SpecialType.System_String && method.Name == "Format")
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, invocation.GetLocation()));
        }
    }

    /// <summary>
    /// Analyzes a binary expression and reports a diagnostic if it is a string concatenation using the + operator.
    /// </summary>
    /// <param name="context">The <see cref="SyntaxNodeAnalysisContext"/> for the current analysis.</param>
    /// <param name="expression">The <see cref="BinaryExpressionSyntax"/> to analyze.</param>
    public static void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context, BinaryExpressionSyntax expression)
    {
        if (!expression.IsKind(SyntaxKind.AddExpression))
        {
            return;
        }

        var type = context.SemanticModel.GetTypeInfo(expression, context.CancellationToken).Type;
        if (type?.SpecialType == SpecialType.System_String)
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, expression.GetLocation()));
        }
    }
}
