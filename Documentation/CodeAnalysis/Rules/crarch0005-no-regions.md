# CRARCH0005 No regions

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0005`

## What it checks

Region directives usually indicate files that need better separation of responsibilities.

## Diagnostic message

`Avoid #region directives`

## How to resolve

Split large types or methods into focused units instead of organizing with `#region`.
