using Discord;
using FFA.Common;
using FFA.Entities.Service;
using FFA.Utility;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class LoggingService : Service
    {
        private readonly SemaphoreSlim _semaphore;

        public LoggingService()
        {
            _semaphore = new SemaphoreSlim(1);

            Directory.CreateDirectory(Config.LOGS_DIRECTORY);
        }

        public async Task LogAsync(LogSeverity severity, string message)
        {
            await _semaphore.WaitAsync();

            try
            {
                var formattedTime = DateTimeOffset.UtcNow.ToString("hh:mm:ss");

                await Console.Out.WriteAsync($"{formattedTime} ");
                await ColoredConsole.WriteAsync($"[{severity}]", GetSeverityColor(severity));
                await Console.Out.WriteLineAsync($" {message}");

                await File.AppendAllTextAsync(LogFileName(severity), $"{formattedTime} [{severity}] {message}\n");
            }
            catch
            {
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public string LogFileName(LogSeverity severity)
            => Config.LOGS_DIRECTORY + DateTimeOffset.UtcNow.ToString("dd'.'MM'.'yyyy") +
               (severity == LogSeverity.Error ? " Errors" : string.Empty) + ".txt";

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
