using System;
using System.Collections.Generic;

namespace FFA.Extensions
{
    public static class RandomExtension
    {
        public static T ArrayElement<T>(this Random random, IReadOnlyList<T> array)
            => array[random.Next(array.Count)];
    }
}
