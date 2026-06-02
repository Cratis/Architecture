# CRARCH0026 Use Cratis Fundamentals metrics

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0026`

## What it checks

Metrics should use Cratis Fundamentals abstractions instead of raw instrument creation.

## Diagnostic message

`Avoid direct Meter.{0} usage`

## How to resolve

Use `IMeter<T>`, `IMeterScope<T>`, and generated metric methods instead of creating meter instruments directly.
