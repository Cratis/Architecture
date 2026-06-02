// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class UseIsNullChecksRule
{
    public const string Id = "CRARCH0008";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Use is null checks", "Use 'is null'/'is not null' instead of '== null'/'!= null'", "Rewrite null checks using pattern matching syntax: is null and is not null.");
}
