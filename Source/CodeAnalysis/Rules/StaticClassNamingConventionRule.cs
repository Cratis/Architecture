// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class StaticClassNamingConventionRule
{
    public const string Id = "CRARCH0015";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Static class naming convention", "Static class '{0}' must end with one of: {1}", "Rename static classes to one of the approved suffixes: Extensions, Converters, Ids, WellKnown, or Defaults.");
}
