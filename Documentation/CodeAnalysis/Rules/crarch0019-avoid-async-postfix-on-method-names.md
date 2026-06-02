# CRARCH0019 Avoid Async postfix on method names

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0019`

## What it checks

Method names should avoid unnecessary suffixes unless sync/async pairs exist.

## Diagnostic message

`Method '{0}' should not end with 'Async' unless a synchronous '{1}' method also exists`

## How to resolve

Rename methods to remove the `Async` suffix unless a synchronous counterpart exists.
