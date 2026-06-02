// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class UseStringInterpolationRule
{
    public const string Id = "CRARCH0009";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Use string interpolation", "Use string interpolation instead of string.Format or concatenation", "Replace string.Format and string concatenation with interpolated strings ($\"...\") for readability.");
}
