// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that forbids injecting IServiceProvider and enforces injection of specific interfaces instead.
/// </summary>
public static class NoIServiceProviderInjectionRule
{
    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0007";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "No IServiceProvider injection", "Do not inject IServiceProvider; inject specific interfaces", "Replace IServiceProvider constructor dependencies with the explicit interfaces required by the type.");

    /// <summary>
    /// Analyzes a constructor parameter and reports a diagnostic if it is of type IServiceProvider.
    /// </summary>
    /// <param name="context">The <see cref="SymbolAnalysisContext"/> for the current analysis.</param>
    /// <param name="parameter">The <see cref="IParameterSymbol"/> to analyze.</param>
    public static void Analyze(SymbolAnalysisContext context, IParameterSymbol parameter)
    {
        if (parameter.Type.ToDisplayString() == "System.IServiceProvider")
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, parameter.Locations.FirstOrDefault()));
        }
    }
}
