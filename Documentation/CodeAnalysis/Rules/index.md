# Rule Reference

This reference lists all diagnostics in `Cratis.Architecture.CodeAnalysis`.

| Rule | Title | Severity | Description |
|---|---|---|---|
| [CRARCH0001](crarch0001-exception-type-naming.md) | Exception type naming | Warning | Exception types must use domain terminology and avoid the generic Exception suffix |
| [CRARCH0002](crarch0002-no-built-in-exception-types.md) | No built-in exception types | Warning | Throwing framework exceptions hides domain intent |
| [CRARCH0003](crarch0003-no-postfixes-on-class-names.md) | No postfixes on class names | Warning | Class names must describe domain concepts, not technical roles |
| [CRARCH0004](crarch0004-no-features-in-namespace.md) | No Features in namespace | Warning | Namespace paths should stay domain-oriented and avoid framework-driven structure names |
| [CRARCH0005](crarch0005-no-regions.md) | No regions | Warning | Region directives usually indicate files that need better separation of responsibilities |
| [CRARCH0006](crarch0006-logging-via-loggermessage.md) | Logging via LoggerMessage | Warning | Structured, source-generated logging ensures consistency and better performance |
| [CRARCH0007](crarch0007-no-iserviceprovider-injection.md) | No IServiceProvider injection | Warning | Service locator patterns hide dependencies and make code harder to reason about |
| [CRARCH0008](crarch0008-use-is-null-checks.md) | Use is null checks | Warning | Pattern matching null checks are the preferred and consistent style |
| [CRARCH0009](crarch0009-use-string-interpolation.md) | Use string interpolation | Warning | Interpolated strings are clearer and easier to maintain |
| [CRARCH0010](crarch0010-constructor-fan-out.md) | Constructor fan-out | Warning | Too many dependencies indicate excessive responsibility in one type |
| [CRARCH0011](crarch0011-file-length-threshold.md) | File length threshold | Warning | Very large files are difficult to understand and maintain |
| [CRARCH0012](crarch0012-async-void-forbidden.md) | async void forbidden | Error | async void methods hide failures and cannot be awaited in regular flows |
| [CRARCH0013](crarch0013-no-blocking-on-async.md) | No blocking on async | Warning | Blocking asynchronous calls can cause deadlocks and reliability issues |
| [CRARCH0014](crarch0014-no-test-types-in-production.md) | No test types in production | Error | Production assemblies must remain independent of test-only infrastructure |
| [CRARCH0015](crarch0015-static-class-naming-convention.md) | Static class naming convention | Warning | Static utility types follow strict naming conventions to improve discoverability |
| [CRARCH0016](crarch0016-unused-interfaces.md) | Unused interfaces | Warning | Speculative interfaces without implementations add unnecessary abstraction |
| [CRARCH0017](crarch0017-namespace-must-align-with-folder-path.md) | Namespace must align with folder path | Warning | Namespace and folder alignment improves navigability and consistency |
| [CRARCH0018](crarch0018-avoid-concrete-type-injection.md) | Avoid concrete type injection | Warning | Constructor dependencies should favor abstractions for loose coupling |
| [CRARCH0019](crarch0019-avoid-async-postfix-on-method-names.md) | Avoid Async postfix on method names | Warning | Method names should avoid unnecessary suffixes unless sync/async pairs exist |
| [CRARCH0020](crarch0020-handle-asynchronous-calls.md) | Handle asynchronous calls | Warning | Fire-and-forget calls can hide failures and produce nondeterministic behavior |
| [CRARCH0021](crarch0021-serializable-attribute-not-allowed.md) | Serializable attribute not allowed | Warning | Legacy serialization attributes are not part of modern Cratis architecture guidance |
| [CRARCH0022](crarch0022-private-modifier-not-allowed.md) | Private modifier not allowed | Warning | Private is implicit in C#, so explicit modifiers add noise |
| [CRARCH0023](crarch0023-use-typed-logger-category.md) | Use typed logger category | Warning | Typed logger categories align log events with the producing type |
| [CRARCH0024](crarch0024-loggermessage-container-conventions.md) | LoggerMessage container conventions | Warning | LoggerMessage methods must live in convention-based containers for consistency |
| [CRARCH0025](crarch0025-use-cratis-fundamentals-traces.md) | Use Cratis Fundamentals traces | Warning | Tracing should flow through Cratis Fundamentals abstractions for consistency |
| [CRARCH0026](crarch0026-use-cratis-fundamentals-metrics.md) | Use Cratis Fundamentals metrics | Warning | Metrics should use Cratis Fundamentals abstractions instead of raw instrument creation |
