using System;

namespace FFA.Extensions
{
    public static class RandomExtension
    {
        public static T ArrayElement<T>(this Random random, T[] array)
            => array[random.Next(array.Length)];
    }
}
