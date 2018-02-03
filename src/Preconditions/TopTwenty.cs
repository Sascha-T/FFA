using Discord.Commands;
using FFA.Database;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Preconditions
{
    public sealed class TopTwenty : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var ffaContext = services.GetRequiredService<FFAContext>();
            var dbGuild = await ffaContext.GetGuildAsync(context.Guild.Id);
            var topTwentyUsers = ffaContext.Users.Where(x => x.GuildId == context.Guild.Id).OrderByDescending(x => x.Reputation).Take(20);

            if (!topTwentyUsers.Any(x => x.Id == context.User.Id))
            {
                return PreconditionResult.FromError("This command may only be used by the top twenty most reputable users.");
            }

            return PreconditionResult.FromSuccess();
        }
    }
}
