// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

public static class NoPostfixesOnClassNamesRule
{
    static readonly string[] _classNameSuffixes = ["Async", "Impl", "Manager", "Helper", "Service"];

    public const string Id = "CRARCH0003";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "No postfixes on class names", "Class '{0}' must not end with postfix '{1}'", "Rename classes to domain concepts and remove technical postfixes such as Async, Impl, Manager, Helper, and Service.");

    public static void Analyze(SymbolAnalysisContext context, INamedTypeSymbol type)
    {
        if (type.TypeKind != TypeKind.Class)
        {
            return;
        }

        foreach (var suffix in _classNameSuffixes)
        {
            if (type.Name.EndsWith(suffix, StringComparison.Ordinal))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, type.Locations.FirstOrDefault(), type.Name, suffix));
                break;
            }
        }
    }
}
