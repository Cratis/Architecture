// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class NoFeaturesInNamespaceRule
{
    public const string Id = "CRARCH0004";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "No Features in namespace", "Namespace '{0}' must not contain '.Features.'", "Remove the Features namespace segment and place the type directly in the domain namespace path.");
}
