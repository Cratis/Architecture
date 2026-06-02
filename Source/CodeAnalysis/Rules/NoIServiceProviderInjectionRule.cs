// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class NoIServiceProviderInjectionRule
{
    public const string Id = "CRARCH0007";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "No IServiceProvider injection", "Do not inject IServiceProvider; inject specific interfaces", "Replace IServiceProvider constructor dependencies with the explicit interfaces required by the type.");
}
