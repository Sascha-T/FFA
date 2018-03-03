using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using FFA.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FFA.Preconditions.Command
{
    public sealed class ErrorLogsAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var logger = services.GetRequiredService<LoggingService>();
            var fileName = logger.LogFileName(LogSeverity.Error);

            if (!File.Exists(fileName))
                return Task.FromResult(PreconditionResult.FromError("No error log file has been created."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
