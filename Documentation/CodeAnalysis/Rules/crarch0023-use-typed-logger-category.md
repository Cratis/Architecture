# CRARCH0023 Use typed logger category

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0023`

## What it checks

Typed logger categories align log events with the producing type.

## Diagnostic message

`Inject ILogger<{0}> instead of '{1}'`

## How to resolve

Inject `ILogger<TContainingType>` instead of non-generic logger forms.
