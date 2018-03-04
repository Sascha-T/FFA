using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using FFA.Common;

namespace FFA.Preconditions.Parameter
{
    public sealed class Muted : ParameterPreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext ctx, ParameterInfo param, object value,
            IServiceProvider services)
        {
            var context = ctx as Context;

            if (value is IGuildUser guildUser && guildUser.RoleIds.Any(x => x == context.DbGuild.MutedRoleId))
                return Task.FromResult(PreconditionResult.FromError("This command may not be used on a muted user."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
