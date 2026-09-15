// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Architecture.CodeAnalysis.Rules;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis;

/// <summary>
/// Analyzer enforcing Cratis architecture diagnostics.
/// </summary>
public partial class ArchitectureAnalyzer
{
    static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is PropertyDeclarationSyntax property)
        {
            AvoidPrimitiveTypesRule.AnalyzeProperty(context, property);
        }
    }

    static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is FieldDeclarationSyntax field)
        {
            AvoidPrimitiveTypesRule.AnalyzeField(context, field);
        }
    }

    static void AnalyzeParameter(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is ParameterSyntax parameter)
        {
            AvoidPrimitiveTypesRule.AnalyzeParameter(context, parameter);
        }
    }
}
