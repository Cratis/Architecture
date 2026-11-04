---
description: "CRARCH0026 asks metrics to use Cratis Fundamentals abstractions instead of creating raw instruments directly."
---

# CRARCH0026 Use Cratis Fundamentals metrics

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0026`

## What it checks

Metrics should use Cratis Fundamentals abstractions instead of raw instrument creation.

## Diagnostic message

`Avoid direct Meter.{0} usage`

## How to resolve

Use `IMeter<T>`, `IMeterScope<T>`, and generated metric methods instead of creating meter instruments directly. Use the typed meter or scope for the reported instrument creation; helper methods may be source generated, but generation is optional. The [C# metrics setup](https://github.com/Cratis/Fundamentals/blob/852fba703f79bef47b9722fc312f6e62b2cf8006/Documentation/csharp/metrics/index.md) explains package references and dependency-injection registration.
