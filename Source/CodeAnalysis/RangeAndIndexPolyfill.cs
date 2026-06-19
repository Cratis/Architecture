// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// The analyzer targets netstandard2.0 so it loads in every compiler host. netstandard2.0 does not
// define System.Index and System.Range, which the C# range and index operators require. These
// minimal polyfills enable the language feature without changing behavior.
#pragma warning disable

namespace System
{
    /// <summary>Represent a type can be used to index a collection either from the start or the end.</summary>
    internal readonly struct Index : IEquatable<Index>
    {
        readonly int _value;

        /// <summary>Initializes a new instance of the <see cref="Index"/> struct.</summary>
        /// <param name="value">The index value. It has to be zero or positive number.</param>
        /// <param name="fromEnd">Indicating if the index is from the start or from the end.</param>
        public Index(int value, bool fromEnd = false)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
            }

            _value = fromEnd ? ~value : value;
        }

        Index(int value) => _value = value;

        /// <summary>Gets an <see cref="Index"/> pointing at first element.</summary>
        public static Index Start => new(0);

        /// <summary>Gets an <see cref="Index"/> pointing at beyond last element.</summary>
        public static Index End => new(~0);

        /// <summary>Gets the index value.</summary>
        public int Value => _value < 0 ? ~_value : _value;

        /// <summary>Gets a value indicating whether the index is from the end.</summary>
        public bool IsFromEnd => _value < 0;

        /// <summary>Create an <see cref="Index"/> from the start at the position indicated by the value.</summary>
        /// <param name="value">The index value from the start.</param>
        /// <returns>The created <see cref="Index"/>.</returns>
        public static Index FromStart(int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
            }

            return new Index(value);
        }

        /// <summary>Create an <see cref="Index"/> from the end at the position indicated by the value.</summary>
        /// <param name="value">The index value from the end.</param>
        /// <returns>The created <see cref="Index"/>.</returns>
        public static Index FromEnd(int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
            }

            return new Index(~value);
        }

        /// <summary>Calculate the offset from the start using the given collection length.</summary>
        /// <param name="length">The length of the collection that the <see cref="Index"/> will be used with.</param>
        /// <returns>The offset.</returns>
        public int GetOffset(int length)
        {
            var offset = _value;
            if (IsFromEnd)
            {
                offset += length + 1;
            }

            return offset;
        }

        /// <summary>Converts integer number to an <see cref="Index"/>.</summary>
        /// <param name="value">The index value from the start.</param>
        public static implicit operator Index(int value) => FromStart(value);

        /// <inheritdoc/>
        public override bool Equals(object? value) => value is Index index && _value == index._value;

        /// <inheritdoc/>
        public bool Equals(Index other) => _value == other._value;

        /// <inheritdoc/>
        public override int GetHashCode() => _value;
    }

    /// <summary>Represent a range that has start and end indexes.</summary>
    internal readonly struct Range : IEquatable<Range>
    {
        /// <summary>Initializes a new instance of the <see cref="Range"/> struct.</summary>
        /// <param name="start">The index representing the start of the range.</param>
        /// <param name="end">The index representing the end of the range.</param>
        public Range(Index start, Index end)
        {
            Start = start;
            End = end;
        }

        /// <summary>Gets the index representing the inclusive start of the range.</summary>
        public Index Start { get; }

        /// <summary>Gets the index representing the exclusive end of the range.</summary>
        public Index End { get; }

        /// <summary>Gets a <see cref="Range"/> that covers the whole collection.</summary>
        public static Range All => new(Index.Start, Index.End);

        /// <summary>Create a <see cref="Range"/> from the given start index to the end of the collection.</summary>
        /// <param name="start">The index representing the start of the range.</param>
        /// <returns>The created <see cref="Range"/>.</returns>
        public static Range StartAt(Index start) => new(start, Index.End);

        /// <summary>Create a <see cref="Range"/> from the start of the collection to the given end index.</summary>
        /// <param name="end">The index representing the end of the range.</param>
        /// <returns>The created <see cref="Range"/>.</returns>
        public static Range EndAt(Index end) => new(Index.Start, end);

        /// <summary>Calculate the start offset and length of the range using the given collection length.</summary>
        /// <param name="length">The length of the collection that the range will be used with.</param>
        /// <returns>The offset and length of the range.</returns>
        public (int Offset, int Length) GetOffsetAndLength(int length)
        {
            var start = Start.IsFromEnd ? length - Start.Value : Start.Value;
            var end = End.IsFromEnd ? length - End.Value : End.Value;

            if ((uint)end > (uint)length || (uint)start > (uint)end)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            return (start, end - start);
        }

        /// <inheritdoc/>
        public override bool Equals(object? value) => value is Range range && range.Start.Equals(Start) && range.End.Equals(End);

        /// <inheritdoc/>
        public bool Equals(Range other) => other.Start.Equals(Start) && other.End.Equals(End);

        /// <inheritdoc/>
        public override int GetHashCode() => (Start.GetHashCode() * 31) + End.GetHashCode();
    }
}

namespace System.Runtime.CompilerServices
{
    /// <summary>Provides runtime helpers required by the range operator on netstandard2.0.</summary>
    internal static class RuntimeHelpers
    {
        /// <summary>Slices the specified array using the specified range.</summary>
        /// <typeparam name="T">The type of the array elements.</typeparam>
        /// <param name="array">The array to slice.</param>
        /// <param name="range">The range to slice with.</param>
        /// <returns>The sliced array.</returns>
        public static T[] GetSubArray<T>(T[] array, Range range)
        {
            var (offset, length) = range.GetOffsetAndLength(array.Length);
            if (length == 0)
            {
                return [];
            }

            var dest = new T[length];
            Array.Copy(array, offset, dest, 0, length);
            return dest;
        }
    }
}
