using Discord.Commands;
using FFA.Common;
using System;
using System.Threading.Tasks;

namespace FFA.Preconditions.Command
{
    public sealed class MemberAge : PreconditionAttribute
    {
        private readonly TimeSpan _timeSpan;

        public MemberAge(int days)
        {
            _timeSpan = TimeSpan.FromDays(days);
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext ctx, CommandInfo command,
            IServiceProvider services)
        {
            var context = ctx as Context;

            if (!context.GuildUser.JoinedAt.HasValue ||
                context.GuildUser.JoinedAt.Value.Add(_timeSpan).CompareTo(DateTimeOffset.UtcNow) > 0)
            {
                return Task.FromResult(PreconditionResult.FromError(
                    $"This command may only be used by members who have been in this guild for at least " +
                    $"{_timeSpan.TotalDays} days."));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
