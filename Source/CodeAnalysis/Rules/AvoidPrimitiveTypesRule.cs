// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that warns about using primitive types directly instead of wrapping them in ConceptAs.
/// </summary>
public static class AvoidPrimitiveTypesRule
{
    static readonly SpecialType[] _primitiveTypes =
    [
        SpecialType.System_String,
        SpecialType.System_Int32,
        SpecialType.System_Int64,
        SpecialType.System_Int16,
        SpecialType.System_Byte,
        SpecialType.System_UInt32,
        SpecialType.System_UInt64,
        SpecialType.System_UInt16,
        SpecialType.System_SByte,
        SpecialType.System_Double,
        SpecialType.System_Single,
        SpecialType.System_Decimal,
        SpecialType.System_Boolean,
    ];

    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0029";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(
            Id,
            "Avoid using primitive types - wrap in ConceptAs<>",
            "{0} '{1}' uses primitive type '{2}' - wrap it in a ConceptAs<{2}> record",
            "Avoid using primitive types such as int, Guid, string directly in domain models, commands, events, or queries. Wrap them using ConceptAs<T> to provide type safety and make the domain model more explicit. For example, create 'public record AuthorId(Guid Value) : ConceptAs<Guid>(Value)' instead of using Guid directly.");

    /// <summary>
    /// Analyzes a property and reports a diagnostic if it uses a primitive type.
    /// </summary>
    /// <param name="context">The <see cref="SyntaxNodeAnalysisContext"/> for the current analysis.</param>
    /// <param name="property">The <see cref="PropertyDeclarationSyntax"/> to analyze.</param>
    public static void AnalyzeProperty(SyntaxNodeAnalysisContext context, PropertyDeclarationSyntax property)
    {
        var semanticModel = context.SemanticModel;
        var typeInfo = semanticModel.GetTypeInfo(property.Type, context.CancellationToken);
        var type = typeInfo.Type;

        if (type is null)
        {
            return;
        }

        // Check if we should warn about this property's type
        if (!ShouldWarnAboutType(context, type, property.Parent?.Parent))
        {
            return;
        }

        var typeName = GetTypeName(type);
        context.ReportDiagnostic(Diagnostic.Create(Descriptor, property.Type.GetLocation(), "Property", property.Identifier.Text, typeName));
    }

    /// <summary>
    /// Analyzes a parameter and reports a diagnostic if it uses a primitive type.
    /// </summary>
    /// <param name="context">The <see cref="SyntaxNodeAnalysisContext"/> for the current analysis.</param>
    /// <param name="parameter">The <see cref="ParameterSyntax"/> to analyze.</param>
    public static void AnalyzeParameter(SyntaxNodeAnalysisContext context, ParameterSyntax parameter)
    {
        if (parameter.Type is null)
        {
            return;
        }

        var semanticModel = context.SemanticModel;
        var typeInfo = semanticModel.GetTypeInfo(parameter.Type, context.CancellationToken);
        var type = typeInfo.Type;

        if (type is null)
        {
            return;
        }

        // Check if we should warn about this parameter's type
        if (!ShouldWarnAboutType(context, type, parameter.Parent?.Parent?.Parent))
        {
            return;
        }

        var typeName = GetTypeName(type);
        context.ReportDiagnostic(Diagnostic.Create(Descriptor, parameter.Type.GetLocation(), "Parameter", parameter.Identifier.Text, typeName));
    }

    /// <summary>
    /// Analyzes a field and reports a diagnostic if it uses a primitive type.
    /// </summary>
    /// <param name="context">The <see cref="SyntaxNodeAnalysisContext"/> for the current analysis.</param>
    /// <param name="field">The <see cref="FieldDeclarationSyntax"/> to analyze.</param>
    public static void AnalyzeField(SyntaxNodeAnalysisContext context, FieldDeclarationSyntax field)
    {
        var semanticModel = context.SemanticModel;
        var typeInfo = semanticModel.GetTypeInfo(field.Declaration.Type, context.CancellationToken);
        var type = typeInfo.Type;

        if (type is null)
        {
            return;
        }

        // Check if we should warn about this field's type
        if (!ShouldWarnAboutType(context, type, field.Parent))
        {
            return;
        }

        var variable = field.Declaration.Variables.FirstOrDefault();
        if (variable is null)
        {
            return;
        }

        var typeName = GetTypeName(type);
        context.ReportDiagnostic(Diagnostic.Create(Descriptor, field.Declaration.Type.GetLocation(), "Field", variable.Identifier.Text, typeName));
    }

    static bool ShouldWarnAboutType(SyntaxNodeAnalysisContext context, ITypeSymbol type, SyntaxNode? containingNode)
    {
        // Check if it's a primitive type or Guid
        var isPrimitive = _primitiveTypes.Contains(type.SpecialType);
        var isGuid = type.Name == "Guid" && type.ContainingNamespace?.ToDisplayString() == "System";

        if (!isPrimitive && !isGuid)
        {
            return false;
        }

        // Don't warn if we're inside a ConceptAs type itself
        if (IsInsideConceptAs(context, containingNode))
        {
            return false;
        }

        // Don't warn if we're in a test file
        if (IsTestFile(context))
        {
            return false;
        }

        // Warn if we're in a class/record that looks like a domain type
        return IsInDomainType(context, containingNode);
    }

    static bool IsInsideConceptAs(SyntaxNodeAnalysisContext context, SyntaxNode? node)
    {
        var typeDeclaration = node;
        while (typeDeclaration != null && typeDeclaration is not TypeDeclarationSyntax)
        {
            typeDeclaration = typeDeclaration.Parent;
        }

        if (typeDeclaration is TypeDeclarationSyntax typeDecl)
        {
            var semanticModel = context.SemanticModel;
            var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, context.CancellationToken);

            if (typeSymbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.BaseType is not null)
            {
                var baseType = namedTypeSymbol.BaseType;
                if (baseType.Name == "ConceptAs" && baseType.IsGenericType)
                {
                    return true;
                }
            }
        }

        return false;
    }

    static bool IsTestFile(SyntaxNodeAnalysisContext context)
    {
        var filePath = context.Node.SyntaxTree.FilePath;
        return filePath.Contains(".Specs", StringComparison.OrdinalIgnoreCase) ||
               filePath.Contains(".Tests", StringComparison.OrdinalIgnoreCase) ||
               filePath.Contains("/test/", StringComparison.OrdinalIgnoreCase) ||
               filePath.Contains("\\test\\", StringComparison.OrdinalIgnoreCase);
    }

    static bool IsInDomainType(SyntaxNodeAnalysisContext context, SyntaxNode? node)
    {
        var typeDeclaration = node;
        while (typeDeclaration != null && typeDeclaration is not TypeDeclarationSyntax)
        {
            typeDeclaration = typeDeclaration.Parent;
        }

        if (typeDeclaration is not TypeDeclarationSyntax typeDecl)
        {
            return false;
        }

        var semanticModel = context.SemanticModel;
        var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, context.CancellationToken);

        if (typeSymbol is null)
        {
            return false;
        }

        // Check for domain-related attributes by checking all attributes
        var attributes = typeSymbol.GetAttributes();
        foreach (var attr in attributes)
        {
            var attrName = attr.AttributeClass?.Name;
            if (attrName is "CommandAttribute" or "Command" or 
                "EventTypeAttribute" or "EventType" or 
                "ReadModelAttribute" or "ReadModel" or 
                "QueryAttribute" or "Query")
            {
                return true;
            }
        }

        // Check for typical domain type naming patterns
        var typeName = typeSymbol.Name;
        if (typeName.EndsWith("Command", StringComparison.Ordinal) ||
            typeName.EndsWith("Event", StringComparison.Ordinal) ||
            typeName.EndsWith("Query", StringComparison.Ordinal) ||
            typeName.EndsWith("ReadModel", StringComparison.Ordinal))
        {
            return true;
        }

        return false;
    }

    static string GetTypeName(ITypeSymbol type)
    {
        if (type.Name == "Guid")
        {
            return "Guid";
        }

        return type.SpecialType switch
        {
            SpecialType.System_String => "string",
            SpecialType.System_Int32 => "int",
            SpecialType.System_Int64 => "long",
            SpecialType.System_Int16 => "short",
            SpecialType.System_Byte => "byte",
            SpecialType.System_UInt32 => "uint",
            SpecialType.System_UInt64 => "ulong",
            SpecialType.System_UInt16 => "ushort",
            SpecialType.System_SByte => "sbyte",
            SpecialType.System_Double => "double",
            SpecialType.System_Single => "float",
            SpecialType.System_Decimal => "decimal",
            SpecialType.System_Boolean => "bool",
            _ => type.Name
        };
    }
}
