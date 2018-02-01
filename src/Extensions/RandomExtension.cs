using System;

namespace FFA.Extensions
{
    public static class RandomExtension
    {
        public static T ArrayElement<T>(this Random random, T[] array)
        {
            return array[random.Next(array.Length)];
        }
    }
}
