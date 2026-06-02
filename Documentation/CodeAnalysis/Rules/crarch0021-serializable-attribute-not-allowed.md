# CRARCH0021 Serializable attribute not allowed

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0021`

## What it checks

Legacy serialization attributes are not part of modern Cratis architecture guidance.

## Diagnostic message

`Type '{0}' must not be marked with [Serializable]`

## How to resolve

Remove `[Serializable]` from types.
