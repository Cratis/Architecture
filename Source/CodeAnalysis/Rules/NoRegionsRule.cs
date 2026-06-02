// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class NoRegionsRule
{
    public const string Id = "CRARCH0005";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "No regions", "Avoid #region directives", "Refactor large files into smaller types or methods instead of organizing code with #region directives.");
}
