// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class ConstructorFanOutRule
{
    public const string Id = "CRARCH0010";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Constructor fan-out", "Constructor has {0} dependencies (maximum is 7)", "Reduce constructor dependencies to seven or fewer by splitting responsibilities or introducing a more focused abstraction.");
}
