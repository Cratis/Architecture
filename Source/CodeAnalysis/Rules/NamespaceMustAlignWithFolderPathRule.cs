// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class NamespaceMustAlignWithFolderPathRule
{
    public const string Id = "CRARCH0017";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "Namespace must align with folder path", "Namespace '{0}' should align with folder path '{1}'", "Adjust the declared namespace or move the file so namespace segments match the folder structure.");
}
