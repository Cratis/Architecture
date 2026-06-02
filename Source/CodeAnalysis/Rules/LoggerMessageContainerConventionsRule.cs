// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class LoggerMessageContainerConventionsRule
{
    public const string Id = "CRARCH0024";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "LoggerMessage container conventions", "Type '{0}' with [LoggerMessage] methods must be an internal static partial *LogMessages class", "Place [LoggerMessage] methods in an internal static partial class named with the LogMessages suffix.");
}
