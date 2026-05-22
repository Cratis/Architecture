using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cratis.Architecture.CodeAnalysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public partial class ArchitectureAnalyzer : DiagnosticAnalyzer
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
}
