---
description: "CRARCH0016 flags interfaces without implementations, because speculative abstractions add needless complexity."
---

# CRARCH0016 Unused interfaces

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0016`

## What it checks

Speculative interfaces without implementations add unnecessary abstraction.

## Diagnostic message

`Interface '{0}' has no concrete implementations`

## How to resolve

Remove unused interfaces or provide concrete implementations where needed.
