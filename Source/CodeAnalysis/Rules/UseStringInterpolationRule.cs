// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

public static class UseStringInterpolationRule
{
    public const string Id = "CRARCH0009";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Use string interpolation", "Use string interpolation instead of string.Format or concatenation", "Replace string.Format and string concatenation with interpolated strings ($\"...\") for readability.");

    public static void AnalyzeInvocation(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, IMethodSymbol method)
    {
        if (method.ContainingType?.SpecialType == SpecialType.System_String && method.Name == "Format")
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, invocation.GetLocation()));
        }
    }

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
