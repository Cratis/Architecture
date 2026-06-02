// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class UseTypedLoggerCategoryRule
{
    public const string Id = "CRARCH0023";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Use typed logger category", "Inject ILogger<{0}> instead of '{1}'", "Use ILogger<TContainingType> for constructor injection so log categories map to the concrete type producing the log.");
}
