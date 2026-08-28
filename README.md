# Cratis Architecture

Roslyn analyzers that turn Cratis architectural conventions into build-time diagnostics for .NET codebases — MIT licensed and free to use.

## Installation

Add the NuGet package to your project:

```bash
dotnet add package Cratis.Architecture.CodeAnalysis
```

Or add it directly to your `.csproj` file:

```xml
<ItemGroup>
  <PackageReference Include="Cratis.Architecture.CodeAnalysis" Version="*" />
</ItemGroup>
```

The analyzers will automatically run during build and provide diagnostics in your IDE.

## What this project represents

This project turns architectural intent into executable rules. Instead of relying on review comments and tribal knowledge, you get immediate diagnostics directly in your editor and CI pipeline.

We treat architecture as a first-class part of developer feedback:

- Architectural boundaries stay visible while you code
- Teams get consistent enforcement across repositories
- New contributors can follow conventions without memorizing all rules

## Catch architectural problems early

Architectural drift gets expensive when it is discovered late. These analyzers are designed to fail fast:

- During local development in your IDE
- During build and test execution
- Before non-compliant code becomes part of your shared history

This shortens feedback loops and keeps architecture aligned with design decisions over time.

## Why this works well with LLMs and agentic development

LLMs and coding agents are much more effective when constraints are explicit and machine-checkable.

Cratis Architecture gives agents and humans the same contract:

- Rules are deterministic and discoverable
- Violations are specific and actionable
- Code fixes are guided by clear architectural diagnostics

That means agent-generated code gets validated against your architectural standards immediately, which reduces rework and keeps automated development aligned with your intended design.

## What the analyzers enforce

26 rules (CRARCH0001–CRARCH0026) covering:

- **Domain-oriented naming** — exception type naming, no technical postfixes on class names, static class naming conventions, domain-oriented namespaces aligned with folder paths
- **Exceptions** — domain-specific exception types instead of built-in framework exceptions
- **Logging and observability** — source-generated `LoggerMessage` logging, typed logger categories, and Cratis Fundamentals traces and metrics
- **Async hygiene** — no `async void`, no blocking on async calls, unawaited calls handled, no `Async` method-name postfix
- **Dependency injection** — no `IServiceProvider` injection, no concrete type injection, constructor fan-out limits
- **Code structure and style** — file length thresholds, no regions, no unused interfaces, no test types in production assemblies, `is null` pattern checks, string interpolation

See the [rule reference](Documentation/CodeAnalysis/Rules/index.md) for every rule with its severity and rationale.

## Documentation

- [Code analysis documentation](Documentation/CodeAnalysis/index.md)
- [Rule reference](Documentation/CodeAnalysis/Rules/index.md)

## The Cratis ecosystem

This project is part of [Cratis](https://www.cratis.io) — free, MIT-licensed tools for building event-sourced and CQRS applications.

- **[Chronicle](https://github.com/Cratis/Chronicle)** — event-sourcing database and runtime. Orleans-based kernel, pluggable storage (MongoDB default; PostgreSQL, SQL Server, SQLite, in-memory), language-agnostic gRPC contracts. [Docs](https://www.cratis.io/chronicle/)
- **Chronicle clients** — first-class [.NET SDK](https://github.com/Cratis/Chronicle), plus [TypeScript](https://github.com/Cratis/Chronicle.TypeScript), [Kotlin/Java](https://github.com/Cratis/Chronicle.Kotlin), and [Elixir](https://github.com/Cratis/Chronicle.Elixir); [Python](https://github.com/Cratis/Chronicle.Python) coming soon (pre-alpha). AI agents connect through the [Chronicle MCP server](https://github.com/Cratis/Chronicle.Mcp).
- **[Arc](https://github.com/Cratis/Arc)** — opinionated CQRS framework for ASP.NET Core with commands, queries, validation, authorization, and TypeScript proxy generation. Works without event sourcing. [Docs](https://www.cratis.io/arc/)
- **[Components](https://github.com/Cratis/Components)** — React components aligned with Arc patterns. [Docs](https://www.cratis.io/components/)
- **[CLI](https://github.com/Cratis/cli) + Workbench** — inspect and diagnose Chronicle from the terminal or the browser. [Docs](https://www.cratis.io/cli/)
- **Model-first layer (experimental)** — [Studio](https://github.com/Cratis/Studio), [Screenplay](https://github.com/Cratis/Screenplay), [Stage](https://github.com/Cratis/Stage), [Scene](https://github.com/Cratis/Scene), [Prologue](https://github.com/Cratis/Prologue)
- **Supporting** — [Fundamentals](https://github.com/Cratis/Fundamentals), [Specifications](https://github.com/Cratis/Specifications), [Synopsis](https://github.com/Cratis/Synopsis), [Lens](https://github.com/Cratis/Lens), [Narrator](https://github.com/Cratis/Narrator), and free [AI tooling](https://github.com/Cratis/AI) (preview); [Ensemble](https://github.com/Cratis/Ensemble) coming soon (pre-release)
- **[Samples](https://github.com/Cratis/Samples)** — runnable event sourcing and CQRS samples for the whole stack

Everything Cratis publishes today is MIT licensed and free to use.
