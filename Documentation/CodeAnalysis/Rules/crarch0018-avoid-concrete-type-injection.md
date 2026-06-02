# CRARCH0018 Avoid concrete type injection

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0018`

## What it checks

Constructor dependencies should favor abstractions for loose coupling.

## Diagnostic message

`Constructor dependency '{0}' should be an interface abstraction`

## How to resolve

Inject interfaces instead of concrete classes. Types marked with `[ReadModel]` are exempt.
