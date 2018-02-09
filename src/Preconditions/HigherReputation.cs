using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Database;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FFA.Preconditions
{
    public sealed class HigherReputation : ParameterPreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services)
        {
            var ffaContext = services.GetRequiredService<FFAContext>();
            var dbUser = await ffaContext.GetUserAsync(value as IGuildUser);

            if (((Context)context).DbUser.Reputation > dbUser.Reputation)
            {
                return PreconditionResult.FromError("You may not use this command on users with a higher reputation than yourself.");
            }

            return PreconditionResult.FromSuccess();
        }
    }
}
