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
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var ffaContext = services.GetRequiredService<FFAContext>();
            var topTwentyUsers = ffaContext.Users.OrderByDescending((x) => x.Reputation).Take(20);

            if (!topTwentyUsers.Any((x) => x.Id == context.User.Id))
            {
                return Task.FromResult(PreconditionResult.FromError("This command may only be used by the top twenty most reputable users."));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
