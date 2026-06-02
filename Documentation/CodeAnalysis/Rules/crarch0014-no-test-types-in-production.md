# CRARCH0014 No test types in production

- **Category:** Architecture
- **Default severity:** Error
- **Diagnostic ID:** `CRARCH0014`

## What it checks

Production assemblies must remain independent of test-only infrastructure.

## Diagnostic message

`Production code must not reference testing/specification types: '{0}'`

## How to resolve

Remove references to testing and specification types from production code.
