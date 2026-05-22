using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Cratis.Architecture.CodeAnalysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ArchitectureAnalyzer : DiagnosticAnalyzer
{
    static readonly string[] ClassNameSuffixes = ["Async", "Impl", "Manager", "Helper", "Service"];
    static readonly string[] StaticClassNameSuffixes = ["Extensions", "Converters", "Ids", "WellKnown", "Defaults"];
    static readonly ImmutableHashSet<string> BuiltInExceptions =
    [
        "System.Exception",
        "System.InvalidOperationException",
        "System.ArgumentException",
        "System.ArgumentNullException",
        "System.ArgumentOutOfRangeException",
        "System.NotImplementedException",
        "System.NotSupportedException",
        "System.ApplicationException",
        "System.NullReferenceException"
    ];

    static readonly DiagnosticDescriptor Rule0001 = CreateRule("CRARCH0001", "Exception type naming", "Exception type '{0}' must not end with 'Exception'", "Rename exception types to domain terms without the Exception suffix, for example AuthorNotFound instead of AuthorNotFoundException.");
    static readonly DiagnosticDescriptor Rule0002 = CreateRule("CRARCH0002", "No built-in exception types", "Throw custom domain exceptions instead of '{0}'", "Replace thrown framework exceptions with domain-specific exception types that express business intent.");
    static readonly DiagnosticDescriptor Rule0003 = CreateRule("CRARCH0003", "No postfixes on class names", "Class '{0}' must not end with postfix '{1}'", "Rename classes to domain concepts and remove technical postfixes such as Async, Impl, Manager, Helper, and Service.");
    static readonly DiagnosticDescriptor Rule0004 = CreateRule("CRARCH0004", "No .Features. in namespace", "Namespace '{0}' must not contain '.Features.'", "Remove the Features namespace segment and place the type directly in the domain namespace path.");
    static readonly DiagnosticDescriptor Rule0005 = CreateRule("CRARCH0005", "No regions", "Avoid #region directives", "Refactor large files into smaller types or methods instead of organizing code with #region directives.");
    static readonly DiagnosticDescriptor Rule0006 = CreateRule("CRARCH0006", "Logging via LoggerMessage", "Use [LoggerMessage] generated methods instead of direct ILogger.Log* calls", "Move logging calls into co-located partial *Logging.cs files and define strongly typed [LoggerMessage] methods.");
    static readonly DiagnosticDescriptor Rule0007 = CreateRule("CRARCH0007", "No IServiceProvider injection", "Do not inject IServiceProvider; inject specific interfaces", "Replace IServiceProvider constructor dependencies with the explicit interfaces required by the type.");
    static readonly DiagnosticDescriptor Rule0008 = CreateRule("CRARCH0008", "Use is null checks", "Use 'is null'/'is not null' instead of '== null'/'!= null'", "Rewrite null checks using pattern matching syntax: is null and is not null.");
    static readonly DiagnosticDescriptor Rule0009 = CreateRule("CRARCH0009", "Use string interpolation", "Use string interpolation instead of string.Format or concatenation", "Replace string.Format and string concatenation with interpolated strings ($\"...\") for readability.");
    static readonly DiagnosticDescriptor Rule0010 = CreateRule("CRARCH0010", "Constructor fan-out", "Constructor has {0} dependencies (maximum is 7)", "Reduce constructor dependencies to seven or fewer by splitting responsibilities or introducing a more focused abstraction.");
    static readonly DiagnosticDescriptor Rule0011 = CreateRule("CRARCH0011", "File length threshold", "File has {0} effective lines (maximum is 400)", "Split large files into smaller focused types once effective code lines exceed 400.");
    static readonly DiagnosticDescriptor Rule0012 = CreateRule("CRARCH0012", "async void forbidden", "Avoid async void methods outside event handlers", "Change async void methods to async Task unless the method is an event handler.", DiagnosticSeverity.Error);
    static readonly DiagnosticDescriptor Rule0013 = CreateRule("CRARCH0013", "No blocking on async", "Avoid blocking async calls via .Result, .Wait(), or .GetAwaiter().GetResult()", "Replace synchronous blocking on asynchronous operations with await to avoid deadlocks and hidden failures.");
    static readonly DiagnosticDescriptor Rule0014 = CreateRule("CRARCH0014", "No test types in production", "Production code must not reference testing/specification types: '{0}'", "Remove references to .Specs/.Testing types from production code and replace them with production abstractions.", DiagnosticSeverity.Error);
    static readonly DiagnosticDescriptor Rule0015 = CreateRule("CRARCH0015", "Static class naming convention", "Static class '{0}' must end with one of: {1}", "Rename static classes to one of the approved suffixes: Extensions, Converters, Ids, WellKnown, or Defaults.");
    static readonly DiagnosticDescriptor Rule0016 = CreateRule("CRARCH0016", "Unused interfaces", "Interface '{0}' has no concrete implementations", "Remove speculative interfaces with no implementations or add a concrete implementation where the abstraction is used.");
    static readonly DiagnosticDescriptor Rule0017 = CreateRule("CRARCH0017", "Namespace must align with folder path", "Namespace '{0}' should align with folder path '{1}'", "Adjust the declared namespace or move the file so namespace segments match the folder structure.");
    static readonly DiagnosticDescriptor Rule0018 = CreateRule("CRARCH0018", "Avoid concrete type injection", "Constructor dependency '{0}' should be an interface abstraction", "Inject interfaces instead of concrete classes. Concrete types marked with [ReadModel] are exempt.");
    static readonly DiagnosticDescriptor Rule0019 = CreateRule("CRARCH0019", "Avoid Async postfix on method names", "Method '{0}' should not end with 'Async' unless a synchronous '{1}' method also exists", "Rename async methods to omit the Async suffix unless the type also exposes an explicit synchronous method with the same base name.");
    static readonly DiagnosticDescriptor Rule0020 = CreateRule("CRARCH0020", "Handle asynchronous calls", "Asynchronous call '{0}' must be handled by awaiting it or chaining a continuation", "Do not fire-and-forget asynchronous calls. Await them or chain a continuation.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
    [
        Rule0001, Rule0002, Rule0003, Rule0004, Rule0005, Rule0006, Rule0007, Rule0008, Rule0009,
        Rule0010, Rule0011, Rule0012, Rule0013, Rule0014, Rule0015, Rule0016, Rule0017, Rule0018, Rule0019, Rule0020
    ];

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

        context.RegisterCompilationStartAction(startContext =>
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
                        endContext.ReportDiagnostic(Diagnostic.Create(Rule0016, location, @interface.Name));
                    }
                }
            });
        });
    }

    static DiagnosticDescriptor CreateRule(string id, string title, string messageFormat, string description, DiagnosticSeverity severity = DiagnosticSeverity.Warning)
        => new(id, title, messageFormat, "Architecture", severity, isEnabledByDefault: true, description: description);

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

    static void AnalyzeNamespace(SyntaxNodeAnalysisContext context)
    {
        var namespaceDeclaration = context.Node switch
        {
            NamespaceDeclarationSyntax ns => ns.Name.ToString(),
            FileScopedNamespaceDeclarationSyntax fs => fs.Name.ToString(),
            _ => string.Empty,
        };

        if (namespaceDeclaration.Contains(".Features.", StringComparison.Ordinal) || namespaceDeclaration.EndsWith(".Features", StringComparison.Ordinal))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule0004, context.Node.GetLocation(), namespaceDeclaration));
        }
    }

    static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
    {
        var root = context.Tree.GetRoot(context.CancellationToken);

        foreach (var trivia in root.DescendantTrivia(descendIntoTrivia: true))
        {
            if (trivia.IsDirective && trivia.GetStructure() is RegionDirectiveTriviaSyntax)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule0005, trivia.GetLocation()));
            }
        }

        var text = context.Tree.GetText(context.CancellationToken);
        var effectiveLines = CountEffectiveLines(text);
        if (effectiveLines > 400)
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule0011, Location.Create(context.Tree, text.Lines[0].Span), effectiveLines));
        }
    }

    static int CountEffectiveLines(SourceText text)
    {
        var count = 0;
        var inBlockComment = false;

        foreach (var line in text.Lines)
        {
            var value = line.ToString().Trim();
            if (value.Length == 0)
            {
                continue;
            }

            if (inBlockComment)
            {
                if (value.Contains("*/", StringComparison.Ordinal))
                {
                    inBlockComment = false;
                }

                continue;
            }

            if (value.StartsWith("//", StringComparison.Ordinal))
            {
                continue;
            }

            if (value.StartsWith("/*", StringComparison.Ordinal))
            {
                if (!value.Contains("*/", StringComparison.Ordinal))
                {
                    inBlockComment = true;
                }

                continue;
            }

            count++;
        }

        return count;
    }

    static void AnalyzeThrow(SyntaxNodeAnalysisContext context)
    {
        ExpressionSyntax? expression = context.Node switch
        {
            ThrowStatementSyntax statement => statement.Expression,
            ThrowExpressionSyntax throwExpression => throwExpression.Expression,
            _ => null,
        };

        if (expression is null)
        {
            return;
        }

        var type = context.SemanticModel.GetTypeInfo(expression, context.CancellationToken).Type;
        var typeName = type?.ToDisplayString();
        if (typeName is not null && BuiltInExceptions.Contains(typeName))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule0002, expression.GetLocation(), type!.Name));
        }
    }

    static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation)
        {
            return;
        }

        if (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is not IMethodSymbol method)
        {
            return;
        }

        var filePath = context.Node.SyntaxTree.FilePath ?? string.Empty;

        if (method.ContainingType?.SpecialType == SpecialType.System_String && method.Name == "Format")
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule0009, invocation.GetLocation()));
        }

        if (!filePath.EndsWith("Logging.cs", StringComparison.Ordinal) &&
            method.Name.StartsWith("Log", StringComparison.Ordinal) &&
            (method.ContainingType?.ToDisplayString().Contains("ILogger", StringComparison.Ordinal) == true ||
             method.ContainingNamespace?.ToDisplayString().Contains("Microsoft.Extensions.Logging", StringComparison.Ordinal) == true))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule0006, invocation.GetLocation()));
        }

        if (method.Name == "Wait" && method.Parameters.Length == 0)
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule0013, invocation.GetLocation()));
        }

        if (method.Name == "GetResult" &&
            method.ContainingType?.Name == "TaskAwaiter" &&
            invocation.Expression is MemberAccessExpressionSyntax { Expression: InvocationExpressionSyntax awaiterInvocation } &&
            context.SemanticModel.GetSymbolInfo(awaiterInvocation, context.CancellationToken).Symbol is IMethodSymbol awaiterMethod &&
            awaiterMethod.Name == "GetAwaiter")
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule0013, invocation.GetLocation()));
        }

        if (ReturnsTaskLike(method.ReturnType) && IsUnhandledAsyncInvocation(invocation, method))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule0020, invocation.GetLocation(), method.Name));
        }
    }

    static void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not BinaryExpressionSyntax expression)
        {
            return;
        }

        if (expression.IsKind(SyntaxKind.EqualsExpression) || expression.IsKind(SyntaxKind.NotEqualsExpression))
        {
            if (expression.Left.IsKind(SyntaxKind.NullLiteralExpression) || expression.Right.IsKind(SyntaxKind.NullLiteralExpression))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule0008, expression.GetLocation()));
            }

            return;
        }

        if (expression.IsKind(SyntaxKind.AddExpression))
        {
            var type = context.SemanticModel.GetTypeInfo(expression, context.CancellationToken).Type;
            if (type?.SpecialType == SpecialType.System_String)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule0009, expression.GetLocation()));
            }
        }
    }

    static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not MethodDeclarationSyntax method)
        {
            return;
        }

        if (method.Modifiers.Any(SyntaxKind.AsyncKeyword) &&
            method.ReturnType is PredefinedTypeSyntax { Keyword.RawKind: (int)SyntaxKind.VoidKeyword } &&
            !IsTestCode(context.Node.SyntaxTree.FilePath) &&
            !IsEventHandler(context, method))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule0012, method.Identifier.GetLocation()));
        }

        if (!IsTestCode(context.Node.SyntaxTree.FilePath) &&
            method.Identifier.ValueText.EndsWith("Async", StringComparison.Ordinal) &&
            context.SemanticModel.GetDeclaredSymbol(method, context.CancellationToken) is IMethodSymbol methodSymbol)
        {
            var baseName = methodSymbol.Name[..^"Async".Length];
            if (baseName.Length == 0 || !HasSynchronousCounterpart(methodSymbol.ContainingType, baseName))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule0019, method.Identifier.GetLocation(), methodSymbol.Name, baseName));
            }
        }
    }

    static bool HasSynchronousCounterpart(INamedTypeSymbol containingType, string baseName)
        => containingType.GetMembers(baseName).OfType<IMethodSymbol>().Any(_ => _.MethodKind == MethodKind.Ordinary && !ReturnsTaskLike(_.ReturnType));

    static bool ReturnsTaskLike(ITypeSymbol returnType)
        => returnType.ToDisplayString() is "System.Threading.Tasks.Task" or "System.Threading.Tasks.ValueTask" ||
           (returnType is INamedTypeSymbol namedType && namedType.IsGenericType &&
            (namedType.ConstructedFrom.ToDisplayString() is "System.Threading.Tasks.Task<T>" or "System.Threading.Tasks.ValueTask<T>"));

    static bool IsUnhandledAsyncInvocation(InvocationExpressionSyntax invocation, IMethodSymbol method)
    {
        if (method.Name == "ContinueWith")
        {
            return false;
        }

        if (invocation.Parent is AwaitExpressionSyntax or ReturnStatementSyntax or ArrowExpressionClauseSyntax)
        {
            return false;
        }

        if (invocation.Parent is MemberAccessExpressionSyntax { Name.Identifier.Text: "ContinueWith" })
        {
            return false;
        }

        return invocation.Parent is ExpressionStatementSyntax;
    }

    static bool IsEventHandler(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method)
    {
        if (method.ParameterList.Parameters.Count != 2 || method.ParameterList.Parameters[0].Type is null || method.ParameterList.Parameters[1].Type is null)
        {
            return false;
        }

        var firstType = context.SemanticModel.GetTypeInfo(method.ParameterList.Parameters[0].Type, context.CancellationToken).Type;
        var secondType = context.SemanticModel.GetTypeInfo(method.ParameterList.Parameters[1].Type, context.CancellationToken).Type as INamedTypeSymbol;

        if (firstType?.SpecialType != SpecialType.System_Object || secondType is null)
        {
            return false;
        }

        while (secondType is not null)
        {
            if (secondType.ToDisplayString() == "System.EventArgs")
            {
                return true;
            }

            secondType = secondType.BaseType;
        }

        return false;
    }

    static void AnalyzeIdentifierTypeUse(SyntaxNodeAnalysisContext context)
    {
        if (IsTestCode(context.Node.SyntaxTree.FilePath))
        {
            return;
        }

        if (context.SemanticModel.GetSymbolInfo(context.Node, context.CancellationToken).Symbol is not ITypeSymbol symbol)
        {
            return;
        }

        var namespaceName = symbol.ContainingNamespace?.ToDisplayString() ?? string.Empty;
        if (namespaceName.Contains(".Specs", StringComparison.Ordinal) || namespaceName.Contains(".Testing", StringComparison.Ordinal))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule0014, context.Node.GetLocation(), symbol.ToDisplayString()));
        }
    }

    static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not MemberAccessExpressionSyntax { Name.Identifier.Text: "Result" } memberAccess)
        {
            return;
        }

        var expressionType = context.SemanticModel.GetTypeInfo(memberAccess.Expression, context.CancellationToken).Type;
        if (expressionType?.ToDisplayString().StartsWith("System.Threading.Tasks.Task", StringComparison.Ordinal) == true)
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule0013, memberAccess.GetLocation()));
        }
    }

    static bool IsTestCode(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return false;
        }

        return filePath.Contains(".Specs/", StringComparison.Ordinal) ||
               filePath.Contains(".Specs\\", StringComparison.Ordinal) ||
               filePath.Contains(".Tests/", StringComparison.Ordinal) ||
               filePath.Contains(".Tests\\", StringComparison.Ordinal);
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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ArchitectureCodeFixProvider))]
public class ArchitectureCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ["CRARCH0008", "CRARCH0009"];

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        foreach (var diagnostic in context.Diagnostics)
        {
            var node = root.FindNode(diagnostic.Location.SourceSpan);

            if (diagnostic.Id == "CRARCH0008" && node is BinaryExpressionSyntax binary)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Use pattern null-check",
                        cancellationToken => UsePatternNullCheckAsync(context.Document, binary, cancellationToken),
                        nameof(ArchitectureCodeFixProvider) + ".CRARCH0008"),
                    diagnostic);
            }

            if (diagnostic.Id == "CRARCH0009" &&
                node is InvocationExpressionSyntax invocation &&
                invocation.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "Format" })
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Use string interpolation",
                        cancellationToken => ConvertStringFormatToInterpolationAsync(context.Document, invocation, cancellationToken),
                        nameof(ArchitectureCodeFixProvider) + ".CRARCH0009"),
                    diagnostic);
            }
        }
    }

    static async Task<Document> UsePatternNullCheckAsync(Document document, BinaryExpressionSyntax expression, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        var nonNullSide = expression.Left.IsKind(SyntaxKind.NullLiteralExpression) ? expression.Right : expression.Left;
        var operatorText = expression.IsKind(SyntaxKind.EqualsExpression) ? "is null" : "is not null";
        var replacement = SyntaxFactory.ParseExpression($"{nonNullSide} {operatorText}").WithTriviaFrom(expression);

        return document.WithSyntaxRoot(root.ReplaceNode(expression, replacement));
    }

    static async Task<Document> ConvertStringFormatToInterpolationAsync(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
    {
        if (invocation.ArgumentList.Arguments.Count == 0)
        {
            return document;
        }

        if (invocation.ArgumentList.Arguments[0].Expression is not LiteralExpressionSyntax { Token.ValueText: var format })
        {
            return document;
        }

        var arguments = invocation.ArgumentList.Arguments.Skip(1).Select(_ => _.Expression.ToString()).ToArray();
        var interpolated = format;
        for (var i = 0; i < arguments.Length; i++)
        {
            interpolated = interpolated.Replace("{" + i + "}", "{" + arguments[i] + "}", StringComparison.Ordinal);
        }

        var replacement = SyntaxFactory.ParseExpression("$\"" + interpolated.Replace("\"", "\\\"", StringComparison.Ordinal) + "\"")
            .WithTriviaFrom(invocation);

        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        return document.WithSyntaxRoot(root.ReplaceNode(invocation, replacement));
    }
}
