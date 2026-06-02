# CRARCH0002 No built-in exception types

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0002`

## What it checks

Throwing framework exceptions hides domain intent.

## Diagnostic message

`Throw custom domain exceptions instead of '{0}'`

## How to resolve

Replace built-in exceptions with custom domain exceptions that describe the business error.
