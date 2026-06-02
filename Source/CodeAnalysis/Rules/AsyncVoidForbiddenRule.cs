// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class AsyncVoidForbiddenRule
{
    public const string Id = "CRARCH0012";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "async void forbidden", "Avoid async void methods outside event handlers", "Change async void methods to async Task unless the method is an event handler.", DiagnosticSeverity.Error);
}
