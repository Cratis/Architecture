---
description: "CRARCH0001 checks C# exception names for domain terminology rather than an Exception suffix, with the diagnostic message and resolution guidance."
---

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
