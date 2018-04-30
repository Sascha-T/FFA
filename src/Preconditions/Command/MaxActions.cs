using Discord.Commands;
using FFA.Common;
using FFA.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FFA.Preconditions.Command
{
    public sealed class MaxActions : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var actionService = services.GetRequiredService<ActionService>();
            var ctx = (Context)context;

            if (!actionService.Authenticate(ctx))
                return Task.FromResult(PreconditionResult.FromError($"You have reached the {ctx.DbGuild.MaxActions} maximum moderation actions per hour."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
