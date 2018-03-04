using Discord.Commands;
using FFA.Common;
using FFA.Entities.Cooldown;
using FFA.Entities.Service;
using FFA.Extensions.Discord;
using FFA.Preconditions.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// TODO: use concurrent bag
namespace FFA.Services
{
    public sealed class CooldownService : Service
    {
        private readonly CommandService _commands;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly IList<Cooldown> _cooldowns = new List<Cooldown>();

        public CooldownService(CommandService commands)
        {
            _commands = commands;
        }

        public async Task<IEnumerable<Cooldown>> GetAllCooldownsAsync(ulong userId, ulong guildId)
        {
            await _semaphore.WaitAsync();

            try
            {
                return _cooldowns.Where(x => x.UserId == userId && x.GuildId == guildId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Cooldown> GetCooldownAsync(ulong userId, ulong guildId, CommandInfo cmd)
        {
            await _semaphore.WaitAsync();

            try
            {
                var cooldown = _cooldowns.FirstOrDefault(x => x.UserId == userId && x.GuildId == guildId && x.Command.Name == cmd.Name);

                if (cooldown == default(Cooldown))
                    return null;
                else if (cooldown.EndsAt.CompareTo(DateTimeOffset.UtcNow) > 0)
                    return cooldown;

                return null;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task ApplyCooldownAsync(Context ctx, int argPos)
        {
            var cmd = _commands.GetCommand(ctx, argPos);
            var cooldownPrecondtion = cmd.Preconditions.FirstOrDefault(x => x is CooldownAttribute) as CooldownAttribute;

            if (cooldownPrecondtion == null)
                return;

            await _semaphore.WaitAsync();

            try
            {
                _cooldowns.Add(new Cooldown(ctx.User.Id, ctx.Guild.Id, cmd, cooldownPrecondtion.CooldownLength));
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
