// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Architecture.CodeAnalysis.Rules;
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
        var expression = context.Node switch
        {
            ThrowStatementSyntax statement => statement.Expression,
            ThrowExpressionSyntax throwExpression => throwExpression.Expression,
            _ => null,
        };

        if (expression is not null)
        {
            NoBuiltInExceptionTypesRule.Analyze(context, expression);
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

        UseStringInterpolationRule.AnalyzeInvocation(context, invocation, method);
        LoggingViaLoggerMessageRule.Analyze(context, invocation, method);
        NoBlockingOnAsyncRule.AnalyzeInvocation(context, invocation, method);
        UseCratisFundamentalsTracesRule.Analyze(context, invocation, method);
        UseCratisFundamentalsMetricsRule.Analyze(context, invocation, method);
        HandleAsynchronousCallsRule.Analyze(context, invocation, method);
    }

    static void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not BinaryExpressionSyntax expression)
        {
            return;
        }

        UseIsNullChecksRule.Analyze(context, expression);
        UseStringInterpolationRule.AnalyzeBinaryExpression(context, expression);
    }
}
