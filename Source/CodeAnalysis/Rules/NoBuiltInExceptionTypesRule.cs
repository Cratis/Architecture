// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that forbids throwing built-in framework exception types in favor of domain-specific exceptions.
/// </summary>
public static class NoBuiltInExceptionTypesRule
{
    static readonly ImmutableHashSet<string> _builtInExceptions =
    [
        "System.Exception",
        "System.InvalidOperationException",
        "System.ArgumentException",
        "System.ArgumentNullException",
        "System.ArgumentOutOfRangeException",
        "System.NotImplementedException",
        "System.NotSupportedException",
        "System.ApplicationException",
        "System.NullReferenceException",
    ];

    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0002";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "No built-in exception types", "Throw custom domain exceptions instead of '{0}'", "Replace thrown framework exceptions with domain-specific exception types that express business intent.");

    /// <summary>
    /// Analyzes an expression and reports a diagnostic if it creates a built-in framework exception type.
    /// </summary>
    /// <param name="context">The <see cref="SyntaxNodeAnalysisContext"/> for the current analysis.</param>
    /// <param name="expression">The <see cref="ExpressionSyntax"/> to analyze.</param>
    public static void Analyze(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
    {
        var type = context.SemanticModel.GetTypeInfo(expression, context.CancellationToken).Type;
        var typeName = type?.ToDisplayString();
        if (typeName is not null && _builtInExceptions.Contains(typeName))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, expression.GetLocation(), type!.Name));
        }
    }
}
