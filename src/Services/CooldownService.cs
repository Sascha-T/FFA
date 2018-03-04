using Discord.Commands;
using FFA.Common;
using FFA.Entities.Cooldown;
using FFA.Entities.Service;
using FFA.Extensions.Discord;
using FFA.Preconditions.Parameter;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FFA.Services
{
    public sealed class CooldownService : Service
    {
        private readonly CommandService _commands;
        private readonly ConcurrentBag<Cooldown> _cooldowns = new ConcurrentBag<Cooldown>();

        public CooldownService(CommandService commands)
        {
            _commands = commands;
        }

        public IReadOnlyList<Cooldown> GetAllCooldowns(ulong userId, ulong guildId)
            => _cooldowns.Where(x => x.UserId == userId && x.GuildId == guildId).ToImmutableArray();

        public Cooldown GetCooldown(ulong userId, ulong guildId, CommandInfo cmd)
        {
            var cooldown = _cooldowns.FirstOrDefault(x => x.UserId == userId && x.GuildId == guildId && x.Command == cmd);

            if (cooldown == default(Cooldown))
                return null;
            else if (cooldown.EndsAt.CompareTo(DateTimeOffset.UtcNow) > 0)
                return cooldown;

            return null;
        }

        public void ApplyCooldown(Context ctx, int argPos)
        {
            var cmd = _commands.GetCommand(ctx, argPos);
            var cooldownPrecondtion = cmd.Preconditions.FirstOrDefault(x => x is CooldownAttribute) as CooldownAttribute;

            if (cooldownPrecondtion == null)
                return;

            var cooldown = _cooldowns.FirstOrDefault(x => x.UserId == ctx.User.Id && x.GuildId == ctx.Guild.Id && x.Command == cmd);

            if (cooldown != default(Cooldown))
                cooldown.EndsAt = DateTimeOffset.UtcNow.Add(cooldownPrecondtion.CooldownLength);
            else
                _cooldowns.Add(new Cooldown(ctx.User.Id, ctx.Guild.Id, cmd, cooldownPrecondtion.CooldownLength));
        }
    }
}
