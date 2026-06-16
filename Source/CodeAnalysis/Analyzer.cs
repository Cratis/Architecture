// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using Cratis.Architecture.CodeAnalysis.Rules;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis;

/// <summary>
/// Analyzer enforcing Cratis architecture diagnostics.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public partial class ArchitectureAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
    [
        ExceptionTypeNamingRule.Descriptor,
        NoBuiltInExceptionTypesRule.Descriptor,
        NoPostfixesOnClassNamesRule.Descriptor,
        NoFeaturesInNamespaceRule.Descriptor,
        NoRegionsRule.Descriptor,
        LoggingViaLoggerMessageRule.Descriptor,
        NoIServiceProviderInjectionRule.Descriptor,
        UseIsNullChecksRule.Descriptor,
        UseStringInterpolationRule.Descriptor,
        ConstructorFanOutRule.Descriptor,
        FileLengthThresholdRule.Descriptor,
        AsyncVoidForbiddenRule.Descriptor,
        NoBlockingOnAsyncRule.Descriptor,
        NoTestTypesInProductionRule.Descriptor,
        StaticClassNamingConventionRule.Descriptor,
        UnusedInterfacesRule.Descriptor,
        NamespaceMustAlignWithFolderPathRule.Descriptor,
        AvoidConcreteTypeInjectionRule.Descriptor,
        AvoidAsyncPostfixOnMethodNamesRule.Descriptor,
        HandleAsynchronousCallsRule.Descriptor,
        SerializableAttributeNotAllowedRule.Descriptor,
        PrivateModifierNotAllowedRule.Descriptor,
        UseTypedLoggerCategoryRule.Descriptor,
        LoggerMessageContainerConventionsRule.Descriptor,
        UseCratisFundamentalsTracesRule.Descriptor,
        UseCratisFundamentalsMetricsRule.Descriptor,
        ConceptAsMustHaveNotSetSentinelRule.Descriptor,
        GuidConceptMustHaveNewFactoryRule.Descriptor,
        AvoidPrimitiveTypesRule.Descriptor,
    ];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        context.RegisterSyntaxNodeAction(AnalyzeNamespace, SyntaxKind.NamespaceDeclaration, SyntaxKind.FileScopedNamespaceDeclaration);
        context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
        context.RegisterSyntaxNodeAction(AnalyzeThrow, SyntaxKind.ThrowStatement, SyntaxKind.ThrowExpression);
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeBinaryExpression, SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression, SyntaxKind.AddExpression);
        context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeIdentifierTypeUse, SyntaxKind.IdentifierName);
        context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
        context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeParameter, SyntaxKind.Parameter);
        context.RegisterSyntaxNodeAction(
            AnalyzePrivateModifier,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.RecordDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.EnumDeclaration,
            SyntaxKind.DelegateDeclaration,
            SyntaxKind.MethodDeclaration,
            SyntaxKind.PropertyDeclaration,
            SyntaxKind.EventDeclaration,
            SyntaxKind.EventFieldDeclaration,
            SyntaxKind.FieldDeclaration,
            SyntaxKind.ConstructorDeclaration,
            SyntaxKind.DestructorDeclaration,
            SyntaxKind.OperatorDeclaration,
            SyntaxKind.ConversionOperatorDeclaration,
            SyntaxKind.IndexerDeclaration,
            SyntaxKind.GetAccessorDeclaration,
            SyntaxKind.SetAccessorDeclaration,
            SyntaxKind.InitAccessorDeclaration,
            SyntaxKind.AddAccessorDeclaration,
            SyntaxKind.RemoveAccessorDeclaration);

        context.RegisterCompilationStartAction(UnusedInterfacesRule.Register);
    }
}
