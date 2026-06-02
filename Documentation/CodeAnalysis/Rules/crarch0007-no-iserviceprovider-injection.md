# CRARCH0007 No IServiceProvider injection

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0007`

## What it checks

Service locator patterns hide dependencies and make code harder to reason about.

## Diagnostic message

`Do not inject IServiceProvider; inject specific interfaces`

## How to resolve

Inject explicit interfaces in constructors instead of `IServiceProvider`.
