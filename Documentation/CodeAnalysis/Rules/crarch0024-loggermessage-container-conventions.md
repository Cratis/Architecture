# CRARCH0024 LoggerMessage container conventions

- **Category:** Architecture
- **Default severity:** Warning
- **Diagnostic ID:** `CRARCH0024`

## What it checks

LoggerMessage methods must live in convention-based containers for consistency.

## Diagnostic message

`Type '{0}' with [LoggerMessage] methods must be an internal static partial *LogMessages class`

## How to resolve

Move `[LoggerMessage]` methods into `internal static partial` classes with a `LogMessages` suffix.
