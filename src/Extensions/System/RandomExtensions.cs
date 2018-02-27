using System;
using System.Collections.Generic;

namespace FFA.Extensions.System
{
    internal static class RandomExtensions
    {
        internal static T ArrayElement<T>(this Random random, IReadOnlyList<T> array)
            => array[random.Next(array.Count)];
    }
}
