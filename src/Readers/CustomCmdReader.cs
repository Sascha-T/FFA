using Discord.Commands;
using FFA.Database.Models;
using FFA.Extensions.Database;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace FFA.Readers
{
    public sealed class CustomCmdReader : TypeReader
    {
        public Type Type { get; } = typeof(CustomCmd);

        public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var dbCustomCmds = services.GetRequiredService<IMongoCollection<CustomCmd>>();
            var customCmd = await dbCustomCmds.FindCustomCmdAsync(input, context.Guild.Id);

            if (customCmd == null)
                return TypeReaderResult.FromError(CommandError.Unsuccessful, "This command does not exist.");

            return TypeReaderResult.FromSuccess(customCmd);
        }
    }
}
