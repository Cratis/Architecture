---
description: "CRARCH0017 requires a type's namespace to align with its folder path, improving navigability and consistency."
---

# CRARCH0017 Namespace must align with folder path

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0017`

## What it checks

The rule derives a logical folder suffix from the file path. It normalizes path separators, finds the first case-sensitive `/Source/` segment, omits the following project-directory segment and the file name, then joins the remaining folders with dots. A missing path, a path without that segment, or a file with no folder beneath the project directory produces an empty suffix. It reports only when the namespace and derived suffix are non-empty, the namespace does not end with that suffix using an ordinal case-sensitive comparison, and a source location is available. Namespace and folder alignment improves navigability and consistency.

## Diagnostic message

`Namespace '{0}' should align with folder path '{1}'`

## How to resolve

Update the namespace or move the file so namespace segments match folder structure.
