# CRARCH0008 Use is null checks

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0008`

## What it checks

Pattern matching null checks are the preferred and consistent style.

## Diagnostic message

`Use 'is null'/'is not null' instead of '== null'/'!= null'`

## How to resolve

Rewrite null checks to use `is null` and `is not null`.
