# CRARCH0006 Logging via LoggerMessage

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0006`

## What it checks

Structured, source-generated logging ensures consistency and better performance.

## Diagnostic message

`Use [LoggerMessage] generated methods instead of direct ILogger.Log* calls`

## How to resolve

Define and use `[LoggerMessage]` methods in `*LogMessages` classes instead of direct log calls.
