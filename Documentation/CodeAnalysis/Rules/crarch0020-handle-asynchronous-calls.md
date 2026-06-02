# CRARCH0020 Handle asynchronous calls

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0020`

## What it checks

Fire-and-forget calls can hide failures and produce nondeterministic behavior.

## Diagnostic message

`Asynchronous call '{0}' must be handled by awaiting it or chaining a continuation`

## How to resolve

Await task-returning calls or explicitly chain a continuation.
