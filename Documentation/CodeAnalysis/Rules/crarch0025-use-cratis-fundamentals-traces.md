---
description: "CRARCH0025 asks tracing to go through Cratis Fundamentals abstractions rather than raw tracing APIs, for consistency."
---

# CRARCH0025 Use Cratis Fundamentals traces

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0025`

## What it checks

Tracing should flow through Cratis Fundamentals abstractions for consistency.

## Diagnostic message

`Avoid direct ActivitySource.StartActivity usage`

## How to resolve

Use `IActivitySource<T>`, `IActivityScope<T>`, and `[Span]` generated methods instead of direct `ActivitySource.StartActivity` calls. Generating span helpers is optional; the typed source and scope are the replacement for the reported raw call. Follow the [C# traces setup](https://github.com/Cratis/Fundamentals/blob/852fba703f79bef47b9722fc312f6e62b2cf8006/Documentation/csharp/traces/index.md) for package and service-registration steps.
