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

        public CooldownAttribute(double hours)
        {
            CooldownLength = TimeSpan.FromHours(hours);
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo cmd, IServiceProvider services)
        {
            var cooldownService = services.GetRequiredService<CooldownService>();
            var cooldown = cooldownService.GetCooldown(context.User.Id, context.Guild.Id, cmd);

            if (cooldown != null)
            {
                var difference = cooldown.EndsAt.Subtract(DateTimeOffset.UtcNow);
                return Task.FromResult(PreconditionResult.FromError($"You may use this command in {difference.ToString(@"hh\:mm\:ss")}."));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
