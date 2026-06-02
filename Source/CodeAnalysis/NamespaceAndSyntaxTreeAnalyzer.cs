// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

using Cratis.Architecture.CodeAnalysis.Rules;

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

        if (namespaceDeclaration.Contains(".Features.", StringComparison.Ordinal) || namespaceDeclaration.EndsWith(".Features", StringComparison.Ordinal))
        {
            context.ReportDiagnostic(Diagnostic.Create(NoFeaturesInNamespaceRule.Descriptor, context.Node.GetLocation(), namespaceDeclaration));
        }
    }

    static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
    {
        var root = context.Tree.GetRoot(context.CancellationToken);

        foreach (var trivia in root.DescendantTrivia(descendIntoTrivia: true))
        {
            if (trivia.IsDirective && trivia.GetStructure() is RegionDirectiveTriviaSyntax)
            {
                context.ReportDiagnostic(Diagnostic.Create(NoRegionsRule.Descriptor, trivia.GetLocation()));
            }
        }

        var text = context.Tree.GetText(context.CancellationToken);
        var effectiveLines = CountEffectiveLines(text);
        if (effectiveLines > 400)
        {
            context.ReportDiagnostic(Diagnostic.Create(FileLengthThresholdRule.Descriptor, Location.Create(context.Tree, text.Lines[0].Span), effectiveLines));
        }
    }

    static int CountEffectiveLines(SourceText text)
    {
        var count = 0;
        var inBlockComment = false;

        foreach (var line in text.Lines)
        {
            var value = line.ToString().Trim();
            if (value.Length == 0)
            {
                continue;
            }

            if (inBlockComment)
            {
                if (value.Contains("*/", StringComparison.Ordinal))
                {
                    inBlockComment = false;
                }

                continue;
            }

            if (value.StartsWith("//", StringComparison.Ordinal))
            {
                continue;
            }

            if (value.StartsWith("/*", StringComparison.Ordinal))
            {
                if (!value.Contains("*/", StringComparison.Ordinal))
                {
                    inBlockComment = true;
                }

                continue;
            }

            count++;
        }

        return count;
    }
}
