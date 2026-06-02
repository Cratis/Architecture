// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class SerializableAttributeNotAllowedRule
{
    public const string Id = "CRARCH0021";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Serializable attribute not allowed", "Type '{0}' must not be marked with [Serializable]", "Remove [Serializable] from types. The attribute is legacy for binary serialization and AppDomain scenarios and should not be used in Cratis code.");
}
