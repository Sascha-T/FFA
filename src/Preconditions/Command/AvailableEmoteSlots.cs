using System;
using System.Threading.Tasks;
using Discord.Commands;
using FFA.Common;

namespace FFA.Preconditions.Command
{
    public sealed class AvailableEmoteSlots : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.Guild.Emotes.Count == Constants.MAX_EMOTES)
                return Task.FromResult(PreconditionResult.FromError($"There are already {Constants.MAX_EMOTES} guild emotes."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
