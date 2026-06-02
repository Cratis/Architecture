# CRARCH0015 Static class naming convention

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0015`

## What it checks

Static utility types follow strict naming conventions to improve discoverability.

## Diagnostic message

`Static class '{0}' must end with one of: {1}`

## How to resolve

Rename static classes to end with `Extensions`, `Converters`, `Ids`, `WellKnown`, or `Defaults`.
