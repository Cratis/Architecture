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
/// Code fix for CRARCH0008.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseIsNullChecksCodeFix))]
public class UseIsNullChecksCodeFix : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds => ["CRARCH0008"];

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
            if (diagnostic.Id == "CRARCH0008" && node is BinaryExpressionSyntax binary)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Use pattern null-check",
                        cancellationToken => UsePatternNullCheckAsync(context.Document, binary, cancellationToken),
                        nameof(UseIsNullChecksCodeFix)),
                    diagnostic);
            }
        }
    }

    private static async Task<Document> UsePatternNullCheckAsync(Document document, BinaryExpressionSyntax expression, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        var nonNullSide = expression.Left.IsKind(SyntaxKind.NullLiteralExpression) ? expression.Right : expression.Left;
        var operatorText = expression.IsKind(SyntaxKind.EqualsExpression) ? "is null" : "is not null";
        var replacement = SyntaxFactory.ParseExpression($"{nonNullSide} {operatorText}").WithTriviaFrom(expression);

        return document.WithSyntaxRoot(root.ReplaceNode(expression, replacement));
    }
}
