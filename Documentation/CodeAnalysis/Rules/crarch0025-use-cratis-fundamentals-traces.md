# CRARCH0025 Use Cratis Fundamentals traces

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0025`

## What it checks

Tracing should flow through Cratis Fundamentals abstractions for consistency.

## Diagnostic message

`Avoid direct ActivitySource.StartActivity usage`

## How to resolve

Use `IActivitySource<T>`, `IActivityScope<T>`, and `[Span]` generated methods instead of direct `ActivitySource.StartActivity` calls.
