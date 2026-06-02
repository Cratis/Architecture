// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

public static class UseCratisFundamentalsTracesRule
{
    public const string Id = "CRARCH0025";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Use Cratis Fundamentals traces", "Avoid direct ActivitySource.StartActivity usage", "Use Cratis Fundamentals trace abstractions (IActivitySource<T>, IActivityScope<T>) and [Span]-generated methods instead of calling ActivitySource.StartActivity directly.");

    public static void Analyze(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, IMethodSymbol method)
    {
        if (method.Name == "StartActivity" && method.ContainingType?.ToDisplayString() == "System.Diagnostics.ActivitySource")
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, invocation.GetLocation()));
        }
    }
}
