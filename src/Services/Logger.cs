using Discord;
using FFA.Utility;
using System;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class Logger
    {
        public async Task LogAsync(LogSeverity severity, string message)
        {
            await Console.Out.WriteAsync($"{DateTime.UtcNow.ToString("hh:mm:ss")} ");
            await ColoredConsole.WriteAsync($"[{severity}]", GetSeverityColor(severity));
            await Console.Out.WriteLineAsync($" {message}");
        }

        private ConsoleColor GetSeverityColor(LogSeverity severity)
        {
            switch (severity)
            {
                case LogSeverity.Critical:
                    return ConsoleColor.Red;
                case LogSeverity.Error:
                    return ConsoleColor.DarkRed;
                case LogSeverity.Info:
                    return ConsoleColor.DarkGreen;
                case LogSeverity.Warning:
                    return ConsoleColor.DarkYellow;
                case LogSeverity.Verbose:
                    return ConsoleColor.DarkCyan;
                case LogSeverity.Debug:
                    return ConsoleColor.DarkMagenta;
                default:
                    return Console.BackgroundColor;
            }
        }
    }
}
