---
description: "CRARCH0010 flags excessive C# constructor dependencies as a sign of too much responsibility and gives guidance for resolving the warning."
---

# CRARCH0010 Constructor fan-out

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0010`

## What it checks

Too many dependencies indicate excessive responsibility in one type.

## Diagnostic message

`Constructor has {0} dependencies (maximum is 7)`

## How to resolve

Refactor to reduce constructor dependencies to seven or fewer. Separate work that needs different dependencies into focused types, or introduce a more focused abstraction for one cohesive responsibility. Recheck each resulting constructor against the same limit.
