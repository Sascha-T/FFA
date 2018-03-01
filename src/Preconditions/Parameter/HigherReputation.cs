using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Database.Models;
using FFA.Extensions.Database;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace FFA.Preconditions.Parameter
{
    public sealed class HigherReputationAttribute : ParameterPreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext ctx, ParameterInfo parameter, object value,
            IServiceProvider services)
        {
            var context = ctx as Context;
            var dbUsers = services.GetRequiredService<IMongoCollection<User>>();
            var dbUser = await dbUsers.GetUserAsync(value as IGuildUser);

            if (context.DbUser.Reputation < dbUser.Reputation)
                return PreconditionResult.FromError("You may not use this command on users with a higher reputation than yourself.");

            return PreconditionResult.FromSuccess();
        }
    }
}
