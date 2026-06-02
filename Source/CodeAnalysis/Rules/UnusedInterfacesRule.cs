// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class UnusedInterfacesRule
{
    public const string Id = "CRARCH0016";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Unused interfaces", "Interface '{0}' has no concrete implementations", "Remove speculative interfaces with no implementations or add a concrete implementation where the abstraction is used.");

    public static void Register(CompilationStartAnalysisContext startContext)
    {
        var interfaceSymbols = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        var implementedInterfaces = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

        startContext.RegisterSymbolAction(symbolContext =>
        {
            if (symbolContext.Symbol is not INamedTypeSymbol type)
            {
                return;
            }

            if (type.TypeKind == TypeKind.Interface)
            {
                interfaceSymbols.Add(type);
                return;
            }

            if (type.TypeKind != TypeKind.Class || type.IsAbstract)
            {
                return;
            }

            foreach (var @interface in type.AllInterfaces)
            {
                implementedInterfaces.Add(@interface);
            }
        }, SymbolKind.NamedType);

        startContext.RegisterCompilationEndAction(endContext =>
        {
            foreach (var @interface in interfaceSymbols)
            {
                if (implementedInterfaces.Contains(@interface) || !@interface.Name.StartsWith('I'))
                {
                    continue;
                }

                var location = @interface.Locations.FirstOrDefault();
                if (location is not null)
                {
                    endContext.ReportDiagnostic(Diagnostic.Create(Descriptor, location, @interface.Name));
                }
            }
        });
    }
}
