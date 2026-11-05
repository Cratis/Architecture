---
description: "CRARCH0012 reports an error for C# async void methods that hide failures and cannot be awaited, with the diagnostic and resolution guidance."
---

# CRARCH0012 async void forbidden

- **Category:** Architecture
- **Default severity:** Error
- **Diagnostic ID:** `CRARCH0012`

## What it checks

`async void` methods hide failures and cannot be awaited in regular flows.

## Diagnostic message

`Avoid async void methods outside event handlers`

## How to resolve

Change `async void` methods to `async Task` unless the method is an event handler.
