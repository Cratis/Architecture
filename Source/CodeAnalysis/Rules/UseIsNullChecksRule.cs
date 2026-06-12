// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that enforces the use of pattern-matching null checks (is null / is not null) over equality operators.
/// </summary>
public static class UseIsNullChecksRule
{
    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0008";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Use is null checks", "Use 'is null'/'is not null' instead of '== null'/'!= null'", "Rewrite null checks using pattern matching syntax: is null and is not null.");

    /// <summary>
    /// Analyzes a binary expression and reports a diagnostic if it uses == null or != null instead of is null or is not null.
    /// </summary>
    /// <param name="context">The <see cref="SyntaxNodeAnalysisContext"/> for the current analysis.</param>
    /// <param name="expression">The <see cref="BinaryExpressionSyntax"/> to analyze.</param>
    public static void Analyze(SyntaxNodeAnalysisContext context, BinaryExpressionSyntax expression)
    {
        if ((expression.IsKind(SyntaxKind.EqualsExpression) || expression.IsKind(SyntaxKind.NotEqualsExpression)) &&
            (expression.Left.IsKind(SyntaxKind.NullLiteralExpression) || expression.Right.IsKind(SyntaxKind.NullLiteralExpression)))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, expression.GetLocation()));
        }
    }
}
