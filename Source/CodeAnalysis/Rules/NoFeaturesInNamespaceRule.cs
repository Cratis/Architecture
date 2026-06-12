// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that enforces namespaces do not contain a Features segment.
/// </summary>
public static class NoFeaturesInNamespaceRule
{
    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0004";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "No Features in namespace", "Namespace '{0}' must not contain '.Features.'", "Remove the Features namespace segment and place the type directly in the domain namespace path.");

    /// <summary>
    /// Analyzes a namespace declaration and reports a diagnostic if it contains a Features segment.
    /// </summary>
    /// <param name="context">The <see cref="SyntaxNodeAnalysisContext"/> for the current analysis.</param>
    /// <param name="namespaceDeclaration">The fully qualified namespace declaration string to analyze.</param>
    public static void Analyze(SyntaxNodeAnalysisContext context, string namespaceDeclaration)
    {
        if (namespaceDeclaration.Contains(".Features.", StringComparison.Ordinal) || namespaceDeclaration.EndsWith(".Features", StringComparison.Ordinal))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, context.Node.GetLocation(), namespaceDeclaration));
        }
    }
}
