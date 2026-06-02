// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class NoBuiltInExceptionTypesRule
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

    public const string Id = "CRARCH0002";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "No built-in exception types", "Throw custom domain exceptions instead of '{0}'", "Replace thrown framework exceptions with domain-specific exception types that express business intent.");

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
