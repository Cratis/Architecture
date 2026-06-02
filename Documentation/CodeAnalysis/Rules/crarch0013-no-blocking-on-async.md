# CRARCH0013 No blocking on async

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0013`

## What it checks

Blocking asynchronous calls can cause deadlocks and reliability issues.

## Diagnostic message

`Avoid blocking async calls via .Result, .Wait(), or .GetAwaiter().GetResult()`

## How to resolve

Use `await` instead of blocking APIs on tasks.
