using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis;

public partial class ArchitectureAnalyzer
{
    static void AnalyzeNamedType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol type)
        {
            return;
        }

        if (type.TypeKind == TypeKind.Class && type.BaseType?.ToDisplayString() == "System.Exception" && type.Name.EndsWith("Exception", StringComparison.Ordinal))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule0001, type.Locations.FirstOrDefault(), type.Name));
        }

        if (type.TypeKind == TypeKind.Class)
        {
            foreach (var suffix in ClassNameSuffixes)
            {
                if (type.Name.EndsWith(suffix, StringComparison.Ordinal))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule0003, type.Locations.FirstOrDefault(), type.Name, suffix));
                    break;
                }
            }
        }

        if (type.TypeKind == TypeKind.Class && type.IsStatic && !StaticClassNameSuffixes.Any(type.Name.EndsWith))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule0015, type.Locations.FirstOrDefault(), type.Name, string.Join(", ", StaticClassNameSuffixes)));
        }

        if (type.TypeKind != TypeKind.Class)
        {
            return;
        }

        foreach (var constructor in type.InstanceConstructors)
        {
            if (constructor.Parameters.Length > 7)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule0010, constructor.Locations.FirstOrDefault(), constructor.Parameters.Length));
            }

            foreach (var parameter in constructor.Parameters)
            {
                if (parameter.Type.ToDisplayString() == "System.IServiceProvider")
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule0007, parameter.Locations.FirstOrDefault()));
                }

                if (ShouldWarnConcreteInjection(parameter.Type))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule0018, parameter.Locations.FirstOrDefault(), parameter.Name));
                }
            }
        }

        var namespaceName = type.ContainingNamespace?.ToDisplayString() ?? string.Empty;
        var location = type.Locations.FirstOrDefault();
        var folderPath = GetLogicalFolderPath(location?.SourceTree?.FilePath);
        if (namespaceName.Length != 0 && folderPath.Length != 0 && !namespaceName.EndsWith(folderPath, StringComparison.Ordinal) && location is not null)
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule0017, location, namespaceName, folderPath));
        }
    }

    static bool ShouldWarnConcreteInjection(ITypeSymbol type)
    {
        if (type is ITypeParameterSymbol)
        {
            return false;
        }

        if (type.TypeKind is TypeKind.Interface or TypeKind.Enum or TypeKind.Struct or TypeKind.Delegate)
        {
            return false;
        }

        if (type.SpecialType != SpecialType.None)
        {
            return false;
        }

        if (type is not INamedTypeSymbol namedType)
        {
            return false;
        }

        if (namedType.Name.EndsWith("Options", StringComparison.Ordinal) || namedType.Name.EndsWith("Settings", StringComparison.Ordinal))
        {
            return false;
        }

        if (namedType.GetAttributes().Any(_ => _.AttributeClass?.Name is "ReadModelAttribute" or "ReadModel"))
        {
            return false;
        }

        if (namedType.ContainingNamespace?.ToDisplayString().StartsWith("System", StringComparison.Ordinal) == true && namedType.IsSealed)
        {
            return false;
        }

        return namedType.TypeKind == TypeKind.Class && !namedType.IsAbstract;
    }

    static string GetLogicalFolderPath(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return string.Empty;
        }

        var normalized = filePath.Replace('\\', '/');
        var sourceIndex = normalized.IndexOf("/Source/", StringComparison.Ordinal);
        if (sourceIndex < 0)
        {
            return string.Empty;
        }

        var relative = normalized[(sourceIndex + "/Source/".Length)..];
        var slashIndex = relative.IndexOf('/');
        if (slashIndex < 0)
        {
            return string.Empty;
        }

        var withinProject = relative[(slashIndex + 1)..];
        var fileNameIndex = withinProject.LastIndexOf('/');
        if (fileNameIndex < 0)
        {
            return string.Empty;
        }

        var folders = withinProject[..fileNameIndex].Split('/', StringSplitOptions.RemoveEmptyEntries);
        return string.Join('.', folders);
    }
}
