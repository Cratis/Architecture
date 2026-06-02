# CRARCH0003 No postfixes on class names

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0003`

## What it checks

Class names must describe domain concepts, not technical roles.

## Diagnostic message

`Class '{0}' must not end with postfix '{1}'`

## How to resolve

Remove postfixes like `Async`, `Impl`, `Manager`, `Helper`, and `Service` from class names.
