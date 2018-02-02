using Discord.Commands;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace FFA.Preconditions
{
    // TODO: no paramter cheap hack
    public sealed class CooldownAttribute : ParameterPreconditionAttribute
    {
        private readonly ConcurrentDictionary<Cooldown, DateTime> _cooldowns = new ConcurrentDictionary<Cooldown, DateTime>();
        private readonly TimeSpan _cooldownLength;

        public CooldownAttribute(int hours)
        {
            _cooldownLength = TimeSpan.FromSeconds(hours);
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services)
        {
            var key = new Cooldown(context.User.Id, parameter.Command.GetHashCode());

            if (_cooldowns.TryGetValue(key, out DateTime endsAt))
            {
                var difference = endsAt.Subtract(DateTime.UtcNow);

                if (difference.Ticks > 0)
                {
                    return Task.FromResult(PreconditionResult.FromError($"You may use this command in {difference.ToString(@"hh\:mm\:ss")}."));
                }

                var time = DateTime.UtcNow.Add(_cooldownLength);

                _cooldowns.TryUpdate(key, time, endsAt);
            }
            else
            {
                _cooldowns.TryAdd(key, DateTime.UtcNow.Add(_cooldownLength));
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
