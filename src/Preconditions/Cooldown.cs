using Discord.Commands;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace FFA.Preconditions
{
    // TODO: no paramter cheap hack
    public sealed class CooldownAttribute : ParameterPreconditionAttribute
    {
        private readonly ConcurrentDictionary<Cooldown, DateTimeOffset> _cooldowns = new ConcurrentDictionary<Cooldown, DateTimeOffset>();
        private readonly TimeSpan _cooldownLength;

        public CooldownAttribute(int hours)
        {
            _cooldownLength = TimeSpan.FromHours(hours);
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services)
        {
            var key = new Cooldown(context.User.Id, parameter.Command.GetHashCode());

            if (_cooldowns.TryGetValue(key, out DateTimeOffset endsAt))
            {
                var difference = endsAt.Subtract(DateTimeOffset.Now);

                if (difference.Ticks > 0)
                {
                    // TODO: Make cooldown response slick?
                    return Task.FromResult(PreconditionResult.FromError($"You may use this command in {difference.ToString(@"hh\:mm\:ss")}."));
                }

                var time = DateTimeOffset.Now.Add(_cooldownLength);

                _cooldowns.TryUpdate(key, time, endsAt);
            }
            else
            {
                _cooldowns.TryAdd(key, DateTimeOffset.Now.Add(_cooldownLength));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }

    public struct Cooldown
    {
        public Cooldown(ulong userId, int commandHash)
        {
            UserId = userId;
            CommandHash = commandHash;
        }

        public ulong UserId { get; }
        public int CommandHash { get; }
    }
}
