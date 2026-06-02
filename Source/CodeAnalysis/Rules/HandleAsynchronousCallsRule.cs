// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class HandleAsynchronousCallsRule
{
    public const string Id = "CRARCH0020";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Handle asynchronous calls", "Asynchronous call '{0}' must be handled by awaiting it or chaining a continuation", "Do not fire-and-forget asynchronous calls. Await them or chain a continuation.");
}
