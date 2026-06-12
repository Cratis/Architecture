// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that forbids the use of #region directives.
/// </summary>
public static class NoRegionsRule
{
    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0005";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "No regions", "Avoid #region directives", "Refactor large files into smaller types or methods instead of organizing code with #region directives.");

    /// <summary>
    /// Analyzes syntax trivia and reports a diagnostic if it is a #region directive.
    /// </summary>
    /// <param name="context">The <see cref="SyntaxTreeAnalysisContext"/> for the current analysis.</param>
    /// <param name="trivia">The <see cref="SyntaxTrivia"/> to analyze.</param>
    public static void Analyze(SyntaxTreeAnalysisContext context, SyntaxTrivia trivia)
    {
        if (trivia.IsDirective && trivia.GetStructure() is RegionDirectiveTriviaSyntax)
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, trivia.GetLocation()));
        }
    }
}
