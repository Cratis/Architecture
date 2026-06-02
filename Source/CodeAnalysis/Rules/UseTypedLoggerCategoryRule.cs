// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

public static class UseTypedLoggerCategoryRule
{
    public const string Id = "CRARCH0023";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Use typed logger category", "Inject ILogger<{0}> instead of '{1}'", "Use ILogger<TContainingType> for constructor injection so log categories map to the concrete type producing the log.");

    public static void Analyze(SymbolAnalysisContext context, INamedTypeSymbol containingType, IParameterSymbol parameter)
    {
        if (IsNonGenericLogger(parameter.Type))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, parameter.Locations.FirstOrDefault(), containingType.Name, "ILogger"));
            return;
        }

        if (TryGetLoggerCategory(parameter.Type, out var categoryType) &&
            categoryType is not null &&
            !SymbolEqualityComparer.Default.Equals(categoryType, containingType))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, parameter.Locations.FirstOrDefault(), containingType.Name, parameter.Type.ToDisplayString()));
        }
    }

    static bool IsNonGenericLogger(ITypeSymbol type)
        => type.ToDisplayString() == "Microsoft.Extensions.Logging.ILogger";

    static bool TryGetLoggerCategory(ITypeSymbol type, out ITypeSymbol? categoryType)
    {
        categoryType = null;

        if (type is not INamedTypeSymbol { IsGenericType: true } namedType)
        {
            return false;
        }

        if (namedType.ConstructedFrom.ToDisplayString() != "Microsoft.Extensions.Logging.ILogger<TCategoryName>")
        {
            return false;
        }

        categoryType = namedType.TypeArguments[0];
        return true;
    }
}
