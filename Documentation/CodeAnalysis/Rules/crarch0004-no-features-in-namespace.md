# CRARCH0004 No Features in namespace

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0004`

## What it checks

Namespace paths should stay domain-oriented and avoid framework-driven structure names.

## Diagnostic message

`Namespace '{0}' must not contain '.Features.'`

## How to resolve

Remove the `.Features.` segment from the namespace and align it with the domain path.
