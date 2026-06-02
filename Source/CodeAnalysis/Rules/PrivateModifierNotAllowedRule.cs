// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

public static class PrivateModifierNotAllowedRule
{
    public const string Id = "CRARCH0022";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Private modifier not allowed", "Avoid explicit 'private' modifier", "Remove explicit private modifiers because private is implicit in C#. Keep explicit private only for property setters.");

    public static void Analyze(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is AccessorDeclarationSyntax accessor)
        {
            var privateModifier = accessor.Modifiers.FirstOrDefault(_ => _.IsKind(SyntaxKind.PrivateKeyword));
            if (privateModifier == default || IsAllowedPrivatePropertySetter(accessor))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, privateModifier.GetLocation()));
            return;
        }

        if (context.Node is not MemberDeclarationSyntax member)
        {
            return;
        }

        var privateKeyword = member.Modifiers.FirstOrDefault(_ => _.IsKind(SyntaxKind.PrivateKeyword));
        if (privateKeyword != default)
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, privateKeyword.GetLocation()));
        }
    }

    static bool IsAllowedPrivatePropertySetter(AccessorDeclarationSyntax accessor)
    {
        if (!accessor.IsKind(SyntaxKind.SetAccessorDeclaration) && !accessor.IsKind(SyntaxKind.InitAccessorDeclaration))
        {
            return false;
        }

        return accessor.Parent?.Parent is BasePropertyDeclarationSyntax property &&
               property.Modifiers.Any(_ => _.IsKind(SyntaxKind.PublicKeyword));
    }
}
