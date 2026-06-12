// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that enforces constructors have no more than seven dependencies.
/// </summary>
public static class ConstructorFanOutRule
{
    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0010";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Constructor fan-out", "Constructor has {0} dependencies (maximum is 7)", "Reduce constructor dependencies to seven or fewer by splitting responsibilities or introducing a more focused abstraction.");

    /// <summary>
    /// Analyzes a constructor symbol and reports a diagnostic if it has more than seven parameters.
    /// </summary>
    /// <param name="context">The <see cref="SymbolAnalysisContext"/> for the current analysis.</param>
    /// <param name="constructor">The <see cref="IMethodSymbol"/> representing the constructor to analyze.</param>
    public static void Analyze(SymbolAnalysisContext context, IMethodSymbol constructor)
    {
        if (constructor.Parameters.Length > 7)
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, constructor.Locations.FirstOrDefault(), constructor.Parameters.Length));
        }
    }
}
