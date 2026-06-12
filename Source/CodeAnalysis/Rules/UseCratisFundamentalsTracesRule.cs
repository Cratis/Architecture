// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that enforces use of Cratis Fundamentals trace abstractions over direct ActivitySource.StartActivity calls.
/// </summary>
public static class UseCratisFundamentalsTracesRule
{
    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0025";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Use Cratis Fundamentals traces", "Avoid direct ActivitySource.StartActivity usage", "Use Cratis Fundamentals trace abstractions (IActivitySource<T>, IActivityScope<T>) and [Span]-generated methods instead of calling ActivitySource.StartActivity directly.");

    /// <summary>
    /// Analyzes an invocation expression and reports a diagnostic if it is a direct ActivitySource.StartActivity call.
    /// </summary>
    /// <param name="context">The <see cref="SyntaxNodeAnalysisContext"/> for the current analysis.</param>
    /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/> to analyze.</param>
    /// <param name="method">The <see cref="IMethodSymbol"/> of the invoked method.</param>
    public static void Analyze(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, IMethodSymbol method)
    {
        if (method.Name == "StartActivity" && method.ContainingType?.ToDisplayString() == "System.Diagnostics.ActivitySource")
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, invocation.GetLocation()));
        }
    }
}
