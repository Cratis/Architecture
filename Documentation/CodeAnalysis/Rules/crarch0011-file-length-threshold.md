---
description: "CRARCH0011 flags files above a length threshold, because very large files are difficult to understand and maintain."
---

# CRARCH0011 File length threshold

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0011`

## What it checks

The text-based counter trims each line, skips blank lines and lines beginning with `//`, and skips a line beginning with `/*` through the first line containing `*/`. Code on skipped lines is not counted; code before an inline comment still counts. It reports only when the resulting count is greater than 400. Very large files are difficult to understand and maintain.

## Diagnostic message

`File has {0} effective lines (maximum is 400)`

## How to resolve

Split the file into smaller focused types or helper abstractions once the threshold is exceeded.
