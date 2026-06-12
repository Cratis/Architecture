// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that enforces logging is done via LoggerMessage-generated methods rather than direct ILogger.Log calls.
/// </summary>
public static class LoggingViaLoggerMessageRule
{
    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0006";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Logging via LoggerMessage", "Use [LoggerMessage] generated methods instead of direct ILogger.Log* calls", "Define log messages as [LoggerMessage] methods in *LogMessages classes and invoke those methods instead of calling ILogger.Log* directly.");

    /// <summary>
    /// Analyzes an invocation expression and reports a diagnostic if it is a direct ILogger.Log call.
    /// </summary>
    /// <param name="context">The <see cref="SyntaxNodeAnalysisContext"/> for the current analysis.</param>
    /// <param name="invocation">The <see cref="InvocationExpressionSyntax"/> to analyze.</param>
    /// <param name="method">The <see cref="IMethodSymbol"/> of the invoked method.</param>
    public static void Analyze(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, IMethodSymbol method)
    {
        if (method.Name.StartsWith("Log", StringComparison.Ordinal) &&
            (method.ContainingType?.ToDisplayString().Contains("ILogger", StringComparison.Ordinal) == true ||
             method.ContainingNamespace?.ToDisplayString().Contains("Microsoft.Extensions.Logging", StringComparison.Ordinal) == true))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, invocation.GetLocation()));
        }
    }
}
