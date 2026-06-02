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
    static void AnalyzeNamespace(SyntaxNodeAnalysisContext context)
    {
        var namespaceDeclaration = context.Node switch
        {
            NamespaceDeclarationSyntax ns => ns.Name.ToString(),
            FileScopedNamespaceDeclarationSyntax fs => fs.Name.ToString(),
            _ => string.Empty,
        };

        NoFeaturesInNamespaceRule.Analyze(context, namespaceDeclaration);
    }

    static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
    {
        var root = context.Tree.GetRoot(context.CancellationToken);

        foreach (var trivia in root.DescendantTrivia(descendIntoTrivia: true))
        {
            NoRegionsRule.Analyze(context, trivia);
        }

        FileLengthThresholdRule.Analyze(context, context.Tree.GetText(context.CancellationToken));
    }
}
