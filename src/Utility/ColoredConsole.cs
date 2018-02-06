using System;
using System.Threading.Tasks;

namespace FFA.Utility
{
    internal static class ColoredConsole
    {
        internal static Task WriteAsync(string message, ConsoleColor color)
        {
            var previous = Console.BackgroundColor;

            Console.BackgroundColor = color;

            var task = Console.Out.WriteAsync(message);

            Console.BackgroundColor = previous;

            return task;
        }

        internal static Task WriteLineAsync(string message, ConsoleColor color)
            => WriteAsync($"{message}\n", color);
    }
}
