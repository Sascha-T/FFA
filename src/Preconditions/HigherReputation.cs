using Discord;
using Discord.Commands;
using FFA.Common;
using System;
using System.Threading.Tasks;

namespace FFA.Preconditions
{
    public sealed class HigherReputation : ParameterPreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext ctx, ParameterInfo parameter, object value, IServiceProvider services)
        {
            var context = ctx as Context;
            var dbUser = await context.Db.GetUserAsync(value as IGuildUser);

            if (context.DbUser.Reputation < dbUser.Reputation)
                return PreconditionResult.FromError("You may not use this command on users with a higher reputation than yourself.");

            return PreconditionResult.FromSuccess();
        }
    }
}
