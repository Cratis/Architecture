// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class StaticClassNamingConventionRule
{
    static readonly string[] _staticClassNameSuffixes = ["Extensions", "Converters", "Ids", "WellKnown", "Defaults"];

    public const string Id = "CRARCH0015";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Static class naming convention", "Static class '{0}' must end with one of: {1}", "Rename static classes to one of the approved suffixes: Extensions, Converters, Ids, WellKnown, or Defaults.");

    public static void Analyze(SymbolAnalysisContext context, INamedTypeSymbol type)
    {
        if (type.TypeKind == TypeKind.Class && type.IsStatic && !_staticClassNameSuffixes.Any(type.Name.EndsWith))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, type.Locations.FirstOrDefault(), type.Name, string.Join(", ", _staticClassNameSuffixes)));
        }
    }
}
