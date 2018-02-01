using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace FFA.Preconditions
{
    public class GuildOnly : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.Guild == null)
            {
                return Task.FromResult(PreconditionResult.FromError("This command may only be used in a guild."));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
