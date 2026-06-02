// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class NoBlockingOnAsyncRule
{
    public const string Id = "CRARCH0013";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "No blocking on async", "Avoid blocking async calls via .Result, .Wait(), or .GetAwaiter().GetResult()", "Replace synchronous blocking on asynchronous operations with await to avoid deadlocks and hidden failures.");
}
