using Discord;
using Discord.Commands;
using FFA.Common;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Preconditions.Parameter
{
    public sealed class NotMutedParamAttribute : ParameterPreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext ctx, ParameterInfo param, object value,
            IServiceProvider services)
        {
            var context = ctx as Context;
            var userVal = value as IUser;
            var guildUser = await ctx.Guild.GetUserAsync(userVal?.Id ?? 0);

            if (guildUser != null && guildUser.RoleIds.Any(x => x == context.DbGuild.MutedRoleId))
                return PreconditionResult.FromError("This command may not be used on a muted user.");

            return PreconditionResult.FromSuccess();
        }
    }
}
