// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class FileLengthThresholdRule
{
    public const string Id = "CRARCH0011";

    public static readonly DiagnosticDescriptor Descriptor =
        DiagnosticRuleFactory.Create(Id, "File length threshold", "File has {0} effective lines (maximum is 400)", "Split large files into smaller focused types once effective code lines exceed 400.");
}
