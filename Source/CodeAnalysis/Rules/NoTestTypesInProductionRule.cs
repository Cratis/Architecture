// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

public static class NoTestTypesInProductionRule
{
    public const string Id = "CRARCH0014";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "No test types in production", "Production code must not reference testing/specification types: '{0}'", "Remove references to .Specs/.Testing types from production code and replace them with production abstractions.", DiagnosticSeverity.Error);

    public static void Analyze(SyntaxNodeAnalysisContext context)
    {
        if (RuleAnalysisUtilities.IsTestCode(context.Node.SyntaxTree.FilePath))
        {
            return;
        }

        if (context.SemanticModel.GetSymbolInfo(context.Node, context.CancellationToken).Symbol is not ITypeSymbol symbol)
        {
            return;
        }

        var namespaceName = symbol.ContainingNamespace?.ToDisplayString() ?? string.Empty;
        if (namespaceName.Contains(".Specs", StringComparison.Ordinal) || namespaceName.Contains(".Testing", StringComparison.Ordinal))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, context.Node.GetLocation(), symbol.ToDisplayString()));
        }
    }
}
