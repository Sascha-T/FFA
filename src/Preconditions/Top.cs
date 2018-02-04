using Discord.Commands;
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

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var repService = services.GetRequiredService<ReputationService>();

            if (!await repService.IsInTop(_count, context.User.Id, context.Guild.Id))
            {
                return PreconditionResult.FromError($"This command may only be used by the top {_count} most reputable users.");
            }

            return PreconditionResult.FromSuccess();
        }
    }
}
