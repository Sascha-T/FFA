using Discord.Commands;
using FFA.Database.Models;
using FFA.Extensions.Database;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Readers
{
    public sealed class RuleReader : TypeReader
    {
        public Type Type { get; } = typeof(Rule);

        public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            // TODO: move to rules service
            // TODO: support more than 9 categories, shouldnt only be 2 chars!
            if (input.Length != 2 || !ushort.TryParse(input[0].ToString(), out ushort categoryNumber))
                return TypeReaderResult.FromError(CommandError.Unsuccessful, "You have provided an invalid rule format.");

            var dbRules = services.GetRequiredService<IMongoCollection<Rule>>();
            var result = await dbRules.WhereAsync(x => x.GuildId == context.Guild.Id);
            var groups = result.OrderBy(x => x.Category).GroupBy(x => x.Category).ToArray();

            if (groups.Length < categoryNumber || categoryNumber <= 0)
                return TypeReaderResult.FromError(CommandError.Unsuccessful, "You have provided an invalid rule category number.");

            var group = groups[categoryNumber - 1].OrderBy(x => x.Content).ToArray();

            input = input.ToLower();

            if (input[1] < 'a' || input[1] > 'a' + group.Length - 1)
                return TypeReaderResult.FromError(CommandError.Unsuccessful, "You have provided an invalid rule letter.");

            return TypeReaderResult.FromSuccess(group[input[1] - 'a']);
        }
    }
}
