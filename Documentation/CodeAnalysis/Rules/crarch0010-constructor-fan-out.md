# CRARCH0010 Constructor fan-out

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0010`

## What it checks

Too many dependencies indicate excessive responsibility in one type.

## Diagnostic message

`Constructor has {0} dependencies (maximum is 7)`

## How to resolve

Refactor to reduce constructor dependencies to seven or fewer.
