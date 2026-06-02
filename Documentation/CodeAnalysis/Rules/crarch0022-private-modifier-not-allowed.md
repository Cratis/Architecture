# CRARCH0022 Private modifier not allowed

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0022`

## What it checks

Private is implicit in C#, so explicit modifiers add noise.

## Diagnostic message

`Avoid explicit 'private' modifier`

## How to resolve

Remove explicit `private` modifiers, except for property setters where it remains allowed.
