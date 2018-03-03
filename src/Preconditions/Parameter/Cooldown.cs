using Discord.Commands;
using FFA.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FFA.Preconditions.Parameter
{
    public sealed class CooldownAttribute : PreconditionAttribute
    {
        public TimeSpan CooldownLength { get; }

        public CooldownAttribute(int hours)
        {
            CooldownLength = TimeSpan.FromHours(hours);
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo cmd, IServiceProvider services)
        {
            var cooldownService = services.GetRequiredService<CooldownService>();
            var cooldown = await cooldownService.GetCooldownAsync(context.User.Id, context.Guild.Id, cmd);

            if (cooldown != null)
            {
                var difference = cooldown.EndsAt.Subtract(DateTimeOffset.UtcNow);
                return PreconditionResult.FromError($"You may use this command in {difference.ToString(@"hh\:mm\:ss")}.");
            }

            return PreconditionResult.FromSuccess();
        }
    }
}
