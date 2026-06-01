// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Specifications;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis.Specs.given;

public class an_architecture_analyzer : Specification
{
    protected IReadOnlyList<Diagnostic> _diagnostics = [];

    protected static async Task<IReadOnlyList<Diagnostic>> analyze(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, path: "/tmp/Source/CodeAnalysis/TestFile.cs");
        var compilation = CSharpCompilation.Create(
            assemblyName: "AnalyzerTests",
            syntaxTrees: [syntaxTree],
            references: get_metadata_references(),
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var analyzer = new ArchitectureAnalyzer();
        var diagnostics = await compilation.WithAnalyzers([analyzer]).GetAnalyzerDiagnosticsAsync().ConfigureAwait(false);
        return diagnostics;
    }


    protected static async Task<string> apply_code_fix(string source, string diagnosticId)
    {
        using var workspace = new AdhocWorkspace();
        var projectId = ProjectId.CreateNewId();
        var documentId = DocumentId.CreateNewId(projectId);

        var solution = workspace.CurrentSolution
            .AddProject(projectId, "AnalyzerTests", "AnalyzerTests", LanguageNames.CSharp)
            .WithProjectCompilationOptions(projectId, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .WithProjectParseOptions(projectId, new CSharpParseOptions(LanguageVersion.Preview));

        foreach (var reference in get_metadata_references())
        {
            solution = solution.AddMetadataReference(projectId, reference);
        }

        solution = solution.AddDocument(documentId, "TestFile.cs", SourceText.From(source));
        var document = solution.GetDocument(documentId)!;

        var compilation = await document.Project.GetCompilationAsync().ConfigureAwait(false);
        if (compilation is null)
        {
            return source;
        }

        var analyzer = new ArchitectureAnalyzer();
        var diagnostics = await compilation.WithAnalyzers([analyzer]).GetAnalyzerDiagnosticsAsync().ConfigureAwait(false);
        var diagnostic = diagnostics.FirstOrDefault(_ => _.Id == diagnosticId);
        if (diagnostic is null)
        {
            return source;
        }

        CodeFixProvider provider = diagnosticId switch
        {
            "CRARCH0008" => new UseIsNullChecksCodeFix(),
            "CRARCH0009" => new UseStringInterpolationCodeFix(),
            _ => throw new InvalidOperationException($"No code fix provider configured for {diagnosticId}."),
        };
        var actions = new List<CodeAction>();

        var context = new CodeFixContext(
            document,
            diagnostic,
            (action, _) => actions.Add(action),
            CancellationToken.None);

        await provider.RegisterCodeFixesAsync(context).ConfigureAwait(false);
        var action = actions.FirstOrDefault();
        if (action is null)
        {
            return source;
        }

        var operations = await action.GetOperationsAsync(CancellationToken.None).ConfigureAwait(false);
        var applyOperation = operations.OfType<ApplyChangesOperation>().FirstOrDefault();
        if (applyOperation is null)
        {
            return source;
        }

        var updatedDocument = applyOperation.ChangedSolution.GetDocument(documentId);
        if (updatedDocument is null)
        {
            return source;
        }

        var updatedText = await updatedDocument.GetTextAsync().ConfigureAwait(false);
        return updatedText.ToString();
    }


    static IReadOnlyList<MetadataReference> get_metadata_references()
    {
        var trustedAssemblies = (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string)?.Split(Path.PathSeparator)
                               ?? [];
        return trustedAssemblies.Select(path => MetadataReference.CreateFromFile(path)).ToList();
    }
}
