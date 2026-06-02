// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class LoggingViaLoggerMessageRule
{
    public const string Id = "CRARCH0006";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Logging via LoggerMessage", "Use [LoggerMessage] generated methods instead of direct ILogger.Log* calls", "Define log messages as [LoggerMessage] methods in *LogMessages classes and invoke those methods instead of calling ILogger.Log* directly.");

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
