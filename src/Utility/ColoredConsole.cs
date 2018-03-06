using System;
using System.Threading.Tasks;

namespace FFA.Utility
{
    public static class ColoredConsole
    {
        public static async Task WriteAsync(string message, ConsoleColor color)
        {
            var previous = Console.BackgroundColor;

            Console.BackgroundColor = color;

            await Console.Out.WriteAsync(message);
            
            Console.BackgroundColor = previous;
        }

        public static Task WriteLineAsync(string message, ConsoleColor color)
            => WriteAsync($"{message}\n", color);
    }
}
