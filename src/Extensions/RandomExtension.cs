using System;
using System.Collections.Immutable;

namespace FFA.Extensions
{
    public static class RandomExtension
    {
        public static T ArrayElement<T>(this Random random, ImmutableArray<T> array)
            => array[random.Next(array.Length)];
    }
}
