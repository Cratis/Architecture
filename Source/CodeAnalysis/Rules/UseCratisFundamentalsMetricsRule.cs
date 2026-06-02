// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class UseCratisFundamentalsMetricsRule
{
    public const string Id = "CRARCH0026";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Use Cratis Fundamentals metrics", "Avoid direct Meter.{0} usage", "Use Cratis Fundamentals metrics abstractions (IMeter<T>, IMeterScope<T>) and [Counter]/[Gauge] generated methods instead of creating Meter instruments directly.");
}
