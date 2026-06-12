// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that reports interfaces that have no concrete implementations.
/// </summary>
public static class UnusedInterfacesRule
{
    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0016";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Unused interfaces", "Interface '{0}' has no concrete implementations", "Remove speculative interfaces with no implementations or add a concrete implementation where the abstraction is used.");

    /// <summary>
    /// Registers symbol and compilation-end actions to detect interfaces with no concrete implementations.
    /// </summary>
    /// <param name="startContext">The <see cref="CompilationStartAnalysisContext"/> used to register compilation-scoped analysis actions.</param>
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
