using Discord.Commands;
using FFA.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FFA.Preconditions.Command
{
    public sealed class TopAttribute : PreconditionAttribute
    {
        private readonly int _count;

        public TopAttribute(int count)
        {
            _count = count;
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext ctx, CommandInfo command, IServiceProvider services)
        {
            var repService = services.GetRequiredService<ReputationService>();

            if (!await repService.IsInTopAsync(_count, ctx.User.Id, ctx.Guild.Id))
                return PreconditionResult.FromError($"This command may only be used by the top {_count} most reputable users.");

            return PreconditionResult.FromSuccess();
        }
    }
}
