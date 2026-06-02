# Cratis Architecture

Cratis Architecture is a Roslyn analyzer package that encodes the architectural conventions we use across Cratis solutions.

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

## Documentation

- [Code analysis documentation](Documentation/CodeAnalysis/index.md)
- [Rule reference](Documentation/CodeAnalysis/Rules/index.md)
