using Discord.Commands;
using FFA.Common;
using FFA.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FFA.Preconditions
{
    public sealed class Top : PreconditionAttribute
    {
        private readonly int _count;

        public Top(int count)
        {
            _count = count;
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext ctx, CommandInfo command, IServiceProvider services)
        {
            var repService = services.GetRequiredService<ReputationService>();
            var context = ctx as Context;

            if (!await repService.IsInTopAsync(context.Db, _count, context.User.Id, context.Guild.Id))
            {
                return PreconditionResult.FromError($"This command may only be used by the top {_count} most reputable users.");
            }

            return PreconditionResult.FromSuccess();
        }
    }
}
