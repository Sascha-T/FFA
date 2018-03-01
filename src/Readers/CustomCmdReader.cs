using Discord.Commands;
using FFA.Database.Models;
using FFA.Extensions.Database;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

// TODO: check guild id when checking for command existance
namespace FFA.Readers
{
    public sealed class CustomCmdReader : TypeReader
    {
        public Type Type { get; } = typeof(CustomCmd);

        public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var dbCustomCmds = services.GetRequiredService<IMongoCollection<CustomCmd>>();
            var lowerInput = input.ToLower();
            var customCmd = await dbCustomCmds.FindOneAsync(x => x.Name == lowerInput);

            if (customCmd == null)
            {
                return TypeReaderResult.FromError(CommandError.ParseFailed, "This command does not exist.");
            }

            return TypeReaderResult.FromSuccess(customCmd);
        }
    }
}
