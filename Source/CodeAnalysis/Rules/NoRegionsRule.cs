// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

public static class NoRegionsRule
{
    public const string Id = "CRARCH0005";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "No regions", "Avoid #region directives", "Refactor large files into smaller types or methods instead of organizing code with #region directives.");

    public static void Analyze(SyntaxTreeAnalysisContext context, SyntaxTrivia trivia)
    {
        if (trivia.IsDirective && trivia.GetStructure() is RegionDirectiveTriviaSyntax)
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, trivia.GetLocation()));
        }
    }
}
