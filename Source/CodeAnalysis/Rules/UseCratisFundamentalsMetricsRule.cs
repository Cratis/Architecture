// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that enforces use of Cratis Fundamentals metrics abstractions over direct Meter instrument creation.
/// </summary>
public static class UseCratisFundamentalsMetricsRule
{
    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0026";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Use Cratis Fundamentals metrics", "Avoid direct Meter.{0} usage", "Use Cratis Fundamentals metrics abstractions (IMeter<T>, IMeterScope<T>) and [Counter]/[Gauge] generated methods instead of creating Meter instruments directly.");

    /// <summary>
    /// Analyzes an invocation expression and reports a diagnostic if it is a direct Meter.Create call.
    /// </summary>
    /// <param name="context">The <see cref="SyntaxNodeAnalysisContext"/> for the current analysis.</param>
    /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/> to analyze.</param>
    /// <param name="method">The <see cref="IMethodSymbol"/> of the invoked method.</param>
    public static void Analyze(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, IMethodSymbol method)
    {
        if (method.Name.StartsWith("Create", StringComparison.Ordinal) && method.ContainingType?.ToDisplayString() == "System.Diagnostics.Metrics.Meter")
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, invocation.GetLocation(), method.Name));
        }
    }
}
