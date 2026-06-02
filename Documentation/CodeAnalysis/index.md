# Code Analysis

Cratis Architecture ships Roslyn analyzers that enforce architectural and coding conventions across Cratis codebases.

## Goals

- Keep architectural intent enforceable
- Surface violations as early as possible
- Give developers and agents clear, actionable feedback

## Rule reference

The analyzer currently provides 26 diagnostics:

- [CRARCH0001](Rules/crarch0001-exception-type-naming.md)
- [CRARCH0002](Rules/crarch0002-no-built-in-exception-types.md)
- [CRARCH0003](Rules/crarch0003-no-postfixes-on-class-names.md)
- [CRARCH0004](Rules/crarch0004-no-features-in-namespace.md)
- [CRARCH0005](Rules/crarch0005-no-regions.md)
- [CRARCH0006](Rules/crarch0006-logging-via-loggermessage.md)
- [CRARCH0007](Rules/crarch0007-no-iserviceprovider-injection.md)
- [CRARCH0008](Rules/crarch0008-use-is-null-checks.md)
- [CRARCH0009](Rules/crarch0009-use-string-interpolation.md)
- [CRARCH0010](Rules/crarch0010-constructor-fan-out.md)
- [CRARCH0011](Rules/crarch0011-file-length-threshold.md)
- [CRARCH0012](Rules/crarch0012-async-void-forbidden.md)
- [CRARCH0013](Rules/crarch0013-no-blocking-on-async.md)
- [CRARCH0014](Rules/crarch0014-no-test-types-in-production.md)
- [CRARCH0015](Rules/crarch0015-static-class-naming-convention.md)
- [CRARCH0016](Rules/crarch0016-unused-interfaces.md)
- [CRARCH0017](Rules/crarch0017-namespace-must-align-with-folder-path.md)
- [CRARCH0018](Rules/crarch0018-avoid-concrete-type-injection.md)
- [CRARCH0019](Rules/crarch0019-avoid-async-postfix-on-method-names.md)
- [CRARCH0020](Rules/crarch0020-handle-asynchronous-calls.md)
- [CRARCH0021](Rules/crarch0021-serializable-attribute-not-allowed.md)
- [CRARCH0022](Rules/crarch0022-private-modifier-not-allowed.md)
- [CRARCH0023](Rules/crarch0023-use-typed-logger-category.md)
- [CRARCH0024](Rules/crarch0024-loggermessage-container-conventions.md)
- [CRARCH0025](Rules/crarch0025-use-cratis-fundamentals-traces.md)
- [CRARCH0026](Rules/crarch0026-use-cratis-fundamentals-metrics.md)
