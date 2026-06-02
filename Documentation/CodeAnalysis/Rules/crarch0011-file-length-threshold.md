# CRARCH0011 File length threshold

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0011`

## What it checks

Very large files are difficult to understand and maintain.

## Diagnostic message

`File has {0} effective lines (maximum is 400)`

## How to resolve

Split the file into smaller focused types or helper abstractions once the threshold is exceeded.
