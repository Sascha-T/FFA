using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using FFA.Common;
using Microsoft.Extensions.DependencyInjection;

namespace FFA.Preconditions
{
    public sealed class BotOwner : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var credentials = services.GetRequiredService<Credentials>();

            if (!credentials.OwnerIds.Any(x => x == context.User.Id))
                return Task.FromResult(PreconditionResult.FromError("This command may only be used by the bot owners."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
