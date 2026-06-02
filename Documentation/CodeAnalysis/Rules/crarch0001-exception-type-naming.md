# CRARCH0001 Exception type naming

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0001`

## What it checks

Exception types must use domain terminology and avoid the generic Exception suffix.

## Diagnostic message

`Exception type '{0}' must not end with 'Exception'`

## How to resolve

Rename the exception type to a domain-focused name, such as `AuthorNotFound` instead of `AuthorNotFoundException`.
