// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

public static class AsyncVoidForbiddenRule
{
    public const string Id = "CRARCH0012";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "async void forbidden", "Avoid async void methods outside event handlers", "Change async void methods to async Task unless the method is an event handler.", DiagnosticSeverity.Error);

    public static void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method)
    {
        if (method.Modifiers.Any(SyntaxKind.AsyncKeyword) &&
            method.ReturnType is PredefinedTypeSyntax { Keyword.RawKind: (int)SyntaxKind.VoidKeyword } &&
            !RuleAnalysisUtilities.IsTestCode(context.Node.SyntaxTree.FilePath) &&
            !IsEventHandler(context, method))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, method.Identifier.GetLocation()));
        }
    }

    static bool IsEventHandler(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method)
    {
        if (method.ParameterList.Parameters.Count != 2)
        {
            return false;
        }

        var firstParameterType = method.ParameterList.Parameters[0].Type;
        var secondParameterType = method.ParameterList.Parameters[1].Type;
        if (firstParameterType is null || secondParameterType is null)
        {
            return false;
        }

        var firstType = context.SemanticModel.GetTypeInfo(firstParameterType, context.CancellationToken).Type;
        var secondType = context.SemanticModel.GetTypeInfo(secondParameterType, context.CancellationToken).Type as INamedTypeSymbol;

        if (firstType?.SpecialType != SpecialType.System_Object || secondType is null)
        {
            return false;
        }

        while (secondType is not null)
        {
            if (secondType.ToDisplayString() == "System.EventArgs")
            {
                return true;
            }

            secondType = secondType.BaseType;
        }

        return false;
    }
}
