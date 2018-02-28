using Discord.Commands;
using FFA.Database.Models;
using FFA.Extensions.Database;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace FFA.Readers
{
    public sealed class CustomCommandTypeReader : TypeReader
    {
        public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var customCommandCollection = services.GetRequiredService<IMongoCollection<CustomCommand>>();
            var lowerInput = input.ToLower();
            var customCmd = await customCommandCollection.FindOneAsync(x => x.Name == lowerInput);

            if (customCmd == default(CustomCommand))
            {
                return TypeReaderResult.FromError(CommandError.ParseFailed, "This command does not exist.");
            }

            return TypeReaderResult.FromSuccess(customCmd);
        }
    }
}
