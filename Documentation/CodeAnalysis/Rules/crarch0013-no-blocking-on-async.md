---
description: "CRARCH0013 flags blocking on asynchronous C# calls and explains how to resolve the warning to avoid deadlocks and reliability problems."
---

# CRARCH0013 No blocking on async

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0013`

## What it checks

Blocking asynchronous calls can cause deadlocks and reliability issues.

## Diagnostic message

`Avoid blocking async calls via .Result, .Wait(), or .GetAwaiter().GetResult()`

## How to resolve

Use `await` instead of blocking APIs on tasks. Keep the containing operation task-returning so its caller can await it, and carry that asynchronous flow through callers that need the result. See [CRARCH0012](crarch0012-async-void-forbidden.md) for return-type guidance and its event-handler exception.
