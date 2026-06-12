// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Rules;

/// <summary>
/// Rule that enforces the declared namespace matches the folder path of the file.
/// </summary>
public static class NamespaceMustAlignWithFolderPathRule
{
    /// <summary>
    /// The diagnostic rule identifier.
    /// </summary>
    public const string Id = "CRARCH0017";

    /// <summary>
    /// The diagnostic descriptor for this rule.
    /// </summary>
    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Namespace must align with folder path", "Namespace '{0}' should align with folder path '{1}'", "Adjust the declared namespace or move the file so namespace segments match the folder structure.");

    /// <summary>
    /// Analyzes a named type symbol and reports a diagnostic if the namespace does not align with the folder path.
    /// </summary>
    /// <param name="context">The <see cref="SymbolAnalysisContext"/> for the current analysis.</param>
    /// <param name="type">The <see cref="INamedTypeSymbol"/> to analyze.</param>
    public static void Analyze(SymbolAnalysisContext context, INamedTypeSymbol type)
    {
        var namespaceName = type.ContainingNamespace?.ToDisplayString() ?? string.Empty;
        var location = type.Locations.FirstOrDefault();
        var folderPath = GetLogicalFolderPath(location?.SourceTree?.FilePath);
        if (namespaceName.Length != 0 && folderPath.Length != 0 && !namespaceName.EndsWith(folderPath, StringComparison.Ordinal) && location is not null)
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, location, namespaceName, folderPath));
        }
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
