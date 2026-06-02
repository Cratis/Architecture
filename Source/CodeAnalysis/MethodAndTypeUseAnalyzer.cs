// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Architecture.CodeAnalysis.Rules;
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

        AsyncVoidForbiddenRule.Analyze(context, method);
        AvoidAsyncPostfixOnMethodNamesRule.Analyze(context, method);
    }

    static void AnalyzeIdentifierTypeUse(SyntaxNodeAnalysisContext context)
        => NoTestTypesInProductionRule.Analyze(context);

    static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is MemberAccessExpressionSyntax memberAccess)
        {
            NoBlockingOnAsyncRule.AnalyzeMemberAccess(context, memberAccess);
        }
    }
}
