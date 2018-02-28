using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace FFA.Preconditions.Command
{
    public sealed class GuildOnlyAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.Guild == null)
                return Task.FromResult(PreconditionResult.FromError("This command may only be used in a guild."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
