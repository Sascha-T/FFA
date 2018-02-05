using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace FFA.Preconditions
{
    public sealed class GuildOwner : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.Guild.OwnerId != context.User.Id)
            {
                return Task.FromResult(PreconditionResult.FromError("This command may only by the guild owner."));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
