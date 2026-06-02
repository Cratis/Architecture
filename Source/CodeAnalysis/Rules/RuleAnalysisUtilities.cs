// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace Cratis.Architecture.CodeAnalysis.Rules;

static class RuleAnalysisUtilities
{
    public static bool IsTestCode(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return false;
        }

        return filePath.Contains(".Specs/", StringComparison.Ordinal) ||
               filePath.Contains(".Specs\\", StringComparison.Ordinal) ||
               filePath.Contains(".Tests/", StringComparison.Ordinal) ||
               filePath.Contains(".Tests\\", StringComparison.Ordinal);
    }

    public static bool ReturnsTaskLike(ITypeSymbol returnType)
        => returnType.ToDisplayString() is "System.Threading.Tasks.Task" or "System.Threading.Tasks.ValueTask" ||
           returnType is INamedTypeSymbol { IsGenericType: true } namedType &&
           namedType.ConstructedFrom.ToDisplayString() is "System.Threading.Tasks.Task<T>" or "System.Threading.Tasks.ValueTask<T>";
}
