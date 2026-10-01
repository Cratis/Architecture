# 0001. Cratis OpenTelemetry convention

- **Status:** Accepted
- **Date:** 2026-10-01
- **Scope:** every Cratis product, library and language client
- **Supersedes:** product-local conventions
- **Related:**
  - [Architecture#33](https://github.com/Cratis/Architecture/issues/33)
  - [Cratis/Arc#2914](https://github.com/Cratis/Arc/issues/2914)
  - [Cratis/Arc#2915](https://github.com/Cratis/Arc/issues/2915)
  - [Cratis/Arc#2919](https://github.com/Cratis/Arc/pull/2919)
  - [Cratis/Arc#2692](https://github.com/Cratis/Arc/issues/2692)
  - [Cratis/Chronicle#4416](https://github.com/Cratis/Chronicle/issues/4416)
  - [Cratis/Chronicle#4417](https://github.com/Cratis/Chronicle/issues/4417)
  - [Cratis/Chronicle#3546](https://github.com/Cratis/Chronicle/issues/3546)
  - [Cratis/AuthProxy#152](https://github.com/Cratis/AuthProxy/issues/152)
  - [Cratis/Documentation#85](https://github.com/Cratis/Documentation/issues/85)
  - [Cratis/AI#461](https://github.com/Cratis/AI/issues/461)
  - [Cratis/Fundamentals#1138](https://github.com/Cratis/Fundamentals/issues/1138)

## Context

Telemetry from the Cratis products does not work together today. A survey found:

- inconsistent scope, span and attribute names across products and languages;
- a hard-coded `service.name` that can override `OTEL_SERVICE_NAME`;
- unconditional OTLP export;
- traces that break at several hops: AuthProxy, Chronicle client to kernel, observers, Wolverine and Dapr;
- raw event source ids in spans;
- a production collector that drops traces.

This record defines one convention so that telemetry from every Cratis product is named, identified and set up the same way, and can be viewed in one place.

## Decision

**D1. Naming**

- **Instrumentation scope** (ActivitySource, Meter, tracer, meter): `Cratis.<Product>[.<Part>]`, **the same string in every language**, with version set to the package version. Language is already visible in `telemetry.sdk.language`.
  - Canonical scopes: `Cratis.Chronicle` (kernel), `Cratis.Chronicle.Client` (.NET, TS, Kotlin, Elixir, Python), `Cratis.Arc`, `Cratis.Arc.MongoDB`, `Cratis.Chronicle.Wolverine`, `Cratis.Chronicle.Dapr`, `Cratis.AuthProxy`, `Cratis.Direct`, `Cratis.Studio`, `Cratis.Stage`, `Cratis.Ensemble`.
  - Every Cratis scope starts with `Cratis.`, so `AddSource("Cratis.*")` and `AddMeter("Cratis.*")` catch all of them.
- **Span names:** `cratis.<product>.<entity>.<operation>`, lowercase, dot-separated, snake_case segments, low cardinality.
  - Domain names (event type, command type, query name) go in attributes, never in the span name. This follows [Cratis/Arc#2919](https://github.com/Cratis/Arc/pull/2919).
  - Client spans become `cratis.chronicle.client.<entity>.<op>`.
  - Span kinds: client calls are Client, kernel RPC is Server, delivery is Consumer, egress is Producer, everything else is Internal.
- **Attributes:**
  - Use OTel semantic conventions wherever one exists: `error.type`, `exception.type`, `rpc.*`, `http.*`, `url.*`, `server.*`, `messaging.*` (Wolverine, Dapr), `db.*` (MongoDB, SQL).
  - Cross-product Cratis concepts go under `cratis.<concept>.<attr>`. Product-only concepts go under `cratis.<product>.<concept>.<attr>`.
  - Enum values are lowercase.
  - Registry:

    | Key | Type | Notes |
    |---|---|---|
    | `cratis.correlation_id` | string (GUID) | |
    | `cratis.tenant.id` | string | Server-side spans only |
    | `cratis.event_store.name` | string | |
    | `cratis.event_store.namespace` | string | |
    | `cratis.event_sequence.id` | string | |
    | `cratis.event_sequence.number` | int | |
    | `cratis.event_type.id` | string | |
    | `cratis.event_type.generation` | int | |
    | `cratis.event_source.type` | string | |
    | `cratis.event_source.id` | string | **Off by default**; see D7 |
    | `cratis.event.count` | int | |
    | `cratis.observer.id` | string | |
    | `cratis.observer.type` | enum | `reactor` \| `reducer` \| `projection` \| `webhook` \| `external` |
    | `cratis.replay` | bool | |
    | `cratis.retry.attempt` | int | |
    | `cratis.arc.command.type` | string | |
    | `cratis.arc.query.name` | string | |
    | `cratis.arc.query.transport` | string | |
    | `cratis.<product>.<x>.outcome` | enum | |

- **Metrics:**
  - Names follow `cratis.<product>.<entity>.<measure>`, dotted, with no unit or `_total` suffix.
  - Every metric has a UCUM unit (`s`, `{event}`) and a description.
  - Durations are histograms in seconds.
  - Metric attributes use the same keys as span attributes and never per-partition, per-event-source or correlation values.
  - Cardinality is capped and overflow is recorded as `_other`, as Arc does with its limit of 1,000.
- **Names are public API.**
  - Each product exposes public constants (`WellKnownTelemetryNames` in .NET, exported consts in TS and Kotlin).
  - A rename ships both names for one minor release, with a changelog entry.
  - One docs page, generated from the constants, lists every name.

**D2. Resource.** Every product sets these:

| Attribute | Source | Rule |
|---|---|---|
| `service.name` | `OTEL_SERVICE_NAME` | The variable wins. Code only supplies a default when it is unset (lowercase, for example `chronicle` or `cratis-authproxy`). Never call `AddService(name)` in a way that overrides the variable. |
| `service.namespace` | `OTEL_RESOURCE_ATTRIBUTES`, or the Kubernetes annotation `resource.opentelemetry.io/service.namespace` | `cratis` for Cratis-operated services. Customer apps choose their own. |
| `service.version` | Set by the library from `AssemblyInformationalVersion`, `package.json` or the Gradle version | |
| `service.instance.id` | Pod name through `OTEL_RESOURCE_ATTRIBUTES`; otherwise the SDK default | Not promoted to Prometheus labels; use `target_info`. |
| `deployment.environment.name` | `OTEL_RESOURCE_ATTRIBUTES`, set by the deployment | Uses the current semconv key. |

Tenant never goes in the resource.

**D3. One setup entry point per platform**

- **.NET:** a new package, **`Cratis.OpenTelemetry`, in the Fundamentals repository**, as an assembly separate from `Cratis.Fundamentals`.
  - API:
    - `IHostApplicationBuilder.AddCratisOpenTelemetry(Action<CratisOpenTelemetryOptions>?)`
    - `OpenTelemetryBuilder.WithCratis()`
    - `TracerProviderBuilder.AddCratisInstrumentation()` and `MeterProviderBuilder.AddCratisInstrumentation()`
  - It registers:
    - logs with `IncludeScopes` and `IncludeFormattedMessage`;
    - `Cratis.*` sources and meters;
    - ASP.NET Core, HttpClient and runtime instrumentation;
    - the resource defaults from D2;
    - `UseOtlpExporter()` only when an OTLP endpoint is configured.
  - Opt-in extras: gRPC client, MongoDB, and the kernel's Orleans sources.
  - **Why Fundamentals and not Arc:**
    - Chronicle client-only hosts (workers, Wolverine, Dapr) don't reference Arc.
    - Fundamentals already owns `IActivitySource<T>` and `[Span]`.
    - Arc, the kernel and AuthProxy all depend on Fundamentals.
    - A separate package keeps ASP.NET Core and OTLP out of Fundamentals core.
  - [Cratis/Arc#2914](https://github.com/Cratis/Arc/issues/2914) is re-scoped to "Arc exposes its names; the Cratis meta-package and templates call `AddCratisOpenTelemetry`".
  - `AddChronicleTelemetry` and `AddCratisChronicleInstrumentation` become `[Obsolete]` forwarders.
- **Node (TS):** libraries depend only on `@opentelemetry/api`. A new `@cratis/opentelemetry` exports `CRATIS_SCOPES` and a thin `startCratisTelemetry()` around `NodeSDK` that honours `OTEL_*`.
- **JVM:** libraries depend only on `opentelemetry-api`. Micrometer Observation is bridged with `micrometer-tracing-bridge-otel`. Setup uses the OTel Java agent or the Spring Boot starter, plus a small `cratis-opentelemetry` auto-configuration for resource defaults.
- **Elixir:** `opentelemetry_api` spans plus `:telemetry` events `[:cratis, :chronicle, :client, …]`. Setup uses the `opentelemetry` and `opentelemetry_exporter` applications.
- **Python:** `opentelemetry-api` only. Setup uses `opentelemetry-instrument` or the distro.

**D4. Configuration.** Only standard OTel configuration:

- Variables: `OTEL_SERVICE_NAME`, `OTEL_RESOURCE_ATTRIBUTES`, `OTEL_EXPORTER_OTLP_{ENDPOINT,PROTOCOL,HEADERS}` and the per-signal variants, `OTEL_TRACES_SAMPLER[_ARG]`, `OTEL_PROPAGATORS`, `OTEL_SDK_DISABLED`, `OTEL_{TRACES,METRICS,LOGS}_EXPORTER`, `OTEL_METRIC_EXPORT_INTERVAL`.
- The same keys in .NET `IConfiguration` (appsettings) are fine.
- No product-specific environment variables or configuration sections for telemetry.
- Code options only for things the OTel spec does not cover, such as hashing event source ids.

**D5. Propagation**

- **Formats:** W3C `tracecontext` and `baggage`, which are the OTel defaults.
- **Every process hop Cratis owns must inject and extract explicitly.** Do not rely on host auto-instrumentation:
  - Arc frontend: optional `traceparent`, plus `X-Correlation-ID`.
  - AuthProxy: spans, and trace context carried through YARP.
  - Chronicle clients: inject into gRPC metadata in every language.
  - Kernel delivery: per [Cratis/Chronicle#4416](https://github.com/Cratis/Chronicle/issues/4416) and [Cratis/Chronicle#4417](https://github.com/Cratis/Chronicle/issues/4417), store the append's W3C trace context with the event as `traceparent`/`tracestate` in the event context, which needs a Chronicle decision record. Observer and handle spans then use an **ActivityLink**, not a parent. Replays and retries are linked and flagged.
  - Wolverine, Dapr and webhook egress: a Producer span linked to the stored context, injected into the transport (CloudEvents `traceparent`).
  - The Wolverine causation `W3CTraceContext/v1` becomes a fallback only.
- **Baggage allow-list:** `cratis.correlation_id` only.
- **Never in baggage:** tenant id, user, subject, identity, event source id, emails, tokens, and namespace names. AuthProxy drops inbound baggage from the internet and strips it on calls to third parties.
- The tenant travels on the one Cratis tenant header and in Orleans `RequestContext`, and is recorded as `cratis.tenant.id` on server spans.

**D6. Correlation id**

- The Chronicle and Arc `CorrelationId` (a GUID) stays the business and audit correlation, persisted with events. The trace id is technical and has a different lifetime: one correlation can span many traces through async reactors, retries and replays. They are **not merged**.
- Every Cratis span and log record in a correlated scope carries `cratis.correlation_id`: through span attributes, through a log scope, and through baggage across non-Cratis hops.
- Arc keeps echoing `X-Correlation-ID`.
- Causation entries become span links once [Cratis/Chronicle#4417](https://github.com/Cratis/Chronicle/issues/4417) lands.

**D7. PII and compliance**

- **Never put these in attributes, events or logs:** payload values of events, commands, queries or read models; `[PII]` or `[NotAudited]` values; identity subject, name or email; tokens; exception messages from non-Cratis code. Record `exception.type` only.
- **Event source id is off by default.** It can be enabled in code, either hashed (HMAC with a deployment key) or raw.
- **.NET logs** classify `EventSourceId`, `Partition` and identity values with `Microsoft.Extensions.Compliance` data classification and enable redaction (a hashing redactor) in production. Other languages use the equivalent in their logging pipeline.

**D8. Defaults**

- **Instrumentation is always on.** It is free when no listener is attached, and there is no Cratis "enable telemetry" switch. The JVM property stays, defaulting to true.
- **Export is opt-in through configuration:** nothing leaves the process unless an OTLP endpoint is set. `OTEL_SDK_DISABLED` is honoured.
- **Templates and Cratis-operated services call the setup by default.**
- **Sampling** defaults to parentbased_always_on. Production sets `OTEL_TRACES_SAMPLER=parentbased_traceidratio`.

**D9. What each signal is for**

- **Traces:** operations and causality (command, append, delivery, handle, publish), with outcome span events.
- **Metrics:** rates, latency, backlog and health: append rate and latency, observer lag, failed and quarantined partitions, command and query duration and outcomes, connected clients. Alerts use metrics or the state APIs, as [Cratis/Infrastructure#93](https://github.com/Cratis/Infrastructure/issues/93) learned.
- **Logs:** discrete diagnostics with `trace_id`/`span_id` (automatic) and the correlation scope. Each record is ingested once.
- Don't log what a span already shows.

**D10. Viewing**

- **Local:** the Aspire dashboard (pinned image, port 18888 for the UI, 4317 for OTLP). Aspire AppHosts in templates set the endpoint automatically.
- **Production (Infrastructure):** Collector → Prometheus for metrics, Loki for logs, **Tempo for traces**, with Grafana linking traces, logs and metrics (`trace_id` derived fields, exemplars).
- **Workbench:** an "Open trace" link on events and failed partitions, built from a configured URL template, plus search by correlation id.
- **Docs:** one Cratis Stack observability page that links to the product pages.

## Consequences

- Every product renames its scopes, spans, metrics and attributes to this convention. A rename ships both names for one minor release (D1).
- A new `Cratis.OpenTelemetry` package in the Fundamentals repository becomes the single .NET setup entry point. `AddChronicleTelemetry` and `AddCratisChronicleInstrumentation` become `[Obsolete]` forwarders (D3).
- Product services stop overriding `OTEL_SERVICE_NAME` and export only when an OTLP endpoint is configured (D2, D8).
- Event source ids leave spans by default and are redacted in logs in production (D7).
- Storing the append's W3C trace context with events needs its own Chronicle decision record (D5).
- Production gets a trace backend (Tempo), and the Workbench links events and failed partitions to their trace (D10).
- Names and attributes are public API, so later changes to them follow the rename rule in D1.

## Where it is followed

- Normative reference: the Cratis Stack observability page ([Cratis/Documentation#85](https://github.com/Cratis/Documentation/issues/85)).
- AI corpus: an observability rule ([Cratis/AI#469](https://github.com/Cratis/AI/issues/469)).
- Mechanical enforcement: analyzers for telemetry names and sensitive tags ([Architecture#34](https://github.com/Cratis/Architecture/issues/34)).
- Product work is tracked in [Architecture#33](https://github.com/Cratis/Architecture/issues/33).
