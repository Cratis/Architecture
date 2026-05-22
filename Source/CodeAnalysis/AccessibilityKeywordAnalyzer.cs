// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis;

/// <summary>
/// Analyzer enforcing Cratis architecture diagnostics.
/// </summary>
public partial class ArchitectureAnalyzer
{
    private static void AnalyzePrivateModifier(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is AccessorDeclarationSyntax accessor)
        {
            var privateModifier = accessor.Modifiers.FirstOrDefault(_ => _.IsKind(SyntaxKind.PrivateKeyword));
            if (privateModifier == default || IsAllowedPrivatePropertySetter(accessor))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(_rule0022, privateModifier.GetLocation()));
            return;
        }

        if (context.Node is not MemberDeclarationSyntax member)
        {
            return;
        }

        var privateKeyword = member.Modifiers.FirstOrDefault(_ => _.IsKind(SyntaxKind.PrivateKeyword));
        if (privateKeyword != default)
        {
            context.ReportDiagnostic(Diagnostic.Create(_rule0022, privateKeyword.GetLocation()));
        }
    }

    private static bool IsAllowedPrivatePropertySetter(AccessorDeclarationSyntax accessor)
    {
        if (!accessor.IsKind(SyntaxKind.SetAccessorDeclaration) && !accessor.IsKind(SyntaxKind.InitAccessorDeclaration))
        {
            return false;
        }

        return accessor.Parent?.Parent is BasePropertyDeclarationSyntax property &&
               property.Modifiers.Any(_ => _.IsKind(SyntaxKind.PublicKeyword));
    }
}
