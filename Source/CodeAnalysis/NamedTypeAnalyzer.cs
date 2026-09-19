// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Architecture.CodeAnalysis.Rules;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis;

/// <summary>
/// Analyzer enforcing Cratis architecture diagnostics.
/// </summary>
public partial class ArchitectureAnalyzer
{
    static void AnalyzeNamedType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol type)
        {
            return;
        }

        SerializableAttributeNotAllowedRule.Analyze(context, type);
        LoggerMessageContainerConventionsRule.Analyze(context, type);
        ExceptionTypeNamingRule.Analyze(context, type);
        NoPostfixesOnClassNamesRule.Analyze(context, type);
        StaticClassNamingConventionRule.Analyze(context, type);
        ConceptAsMustHaveNotSetSentinelRule.Analyze(context, type);
        GuidConceptMustHaveNewFactoryRule.Analyze(context, type);

        if (type.TypeKind != TypeKind.Class)
        {
            return;
        }

        foreach (var constructor in type.InstanceConstructors)
        {
            ConstructorFanOutRule.Analyze(context, constructor);

            foreach (var parameter in constructor.Parameters)
            {
                NoIServiceProviderInjectionRule.Analyze(context, parameter);
                UseTypedLoggerCategoryRule.Analyze(context, type, parameter);
                AvoidConcreteTypeInjectionRule.Analyze(context, parameter);
            }
        }

        NamespaceMustAlignWithFolderPathRule.Analyze(context, type);
    }
}
