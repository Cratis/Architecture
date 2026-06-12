// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that enforces files do not exceed 400 effective lines of code.
/// </summary>
public static class FileLengthThresholdRule
{
    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0011";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "File length threshold", "File has {0} effective lines (maximum is 400)", "Split large files into smaller focused types once effective code lines exceed 400.");

    /// <summary>
    /// Analyzes source text and reports a diagnostic if the effective line count exceeds 400.
    /// </summary>
    /// <param name="context">The <see cref="SyntaxTreeAnalysisContext"/> for the current analysis.</param>
    /// <param name="text">The <see cref="SourceText"/> of the file to analyze.</param>
    public static void Analyze(SyntaxTreeAnalysisContext context, SourceText text)
    {
        var effectiveLines = CountEffectiveLines(text);
        if (effectiveLines > 400)
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, Location.Create(context.Tree, text.Lines[0].Span), effectiveLines));
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
