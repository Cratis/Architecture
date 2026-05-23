// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cratis.Architecture.CodeAnalysis;

/// <summary>
/// Code fix for CRARCH0009.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseStringInterpolationCodeFix))]
public class UseStringInterpolationCodeFix : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds => ["CRARCH0009"];

    /// <inheritdoc/>
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        foreach (var diagnostic in context.Diagnostics)
        {
            var node = root.FindNode(diagnostic.Location.SourceSpan);
            if (diagnostic.Id == "CRARCH0009" &&
                node is InvocationExpressionSyntax invocation &&
                invocation.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "Format" })
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Use string interpolation",
                        cancellationToken => ConvertStringFormatToInterpolationAsync(context.Document, invocation, cancellationToken),
                        nameof(UseStringInterpolationCodeFix)),
                    diagnostic);
            }
        }
    }

    private static async Task<Document> ConvertStringFormatToInterpolationAsync(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
    {
        if (invocation.ArgumentList.Arguments.Count == 0)
        {
            return document;
        }

        if (invocation.ArgumentList.Arguments[0].Expression is not LiteralExpressionSyntax { Token.ValueText: var format })
        {
            return document;
        }

        var arguments = invocation.ArgumentList.Arguments.Skip(1).Select(_ => _.Expression.ToString()).ToArray();
        var interpolated = format;
        for (var i = 0; i < arguments.Length; i++)
        {
            interpolated = interpolated.Replace("{" + i + "}", "{" + arguments[i] + "}", StringComparison.Ordinal);
        }

        var replacement = SyntaxFactory.ParseExpression("$\"" + interpolated.Replace("\"", "\\\"", StringComparison.Ordinal) + "\"")
            .WithTriviaFrom(invocation);

        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        return document.WithSyntaxRoot(root.ReplaceNode(invocation, replacement));
    }
}
