using Discord;
using Discord.WebSocket;
using FFA.Common;
using FFA.Database;
using FFA.Database.Models;
using FFA.Extensions;
using FFA.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FFA.Timers
{
    internal sealed class AutoUnmute
    {
        private readonly IServiceProvider _provider;
        private readonly IDiscordClient _client;
        private readonly ModerationService _moderationService;
        private readonly Timer _timer;
        private readonly AutoResetEvent _autoEvent;

        internal AutoUnmute(IServiceProvider provider)
        {
            _provider = provider;
            _client = _provider.GetRequiredService<DiscordSocketClient>();
            _moderationService = _provider.GetRequiredService<ModerationService>();
            _autoEvent = new AutoResetEvent(false);
            _timer = new Timer(Execute, _autoEvent, 0, Configuration.AUTO_UNMUTE_TIMER);
        }

        private void Execute(object state)
            => Task.Run(async () =>
            {
                var ffaContext = _provider.GetRequiredService<FFAContext>();

                foreach (var guild in await _client.GetGuildsAsync())
                {
                    var dbGuild = await ffaContext.GetGuildAsync(guild.Id);

                    if (!dbGuild.MutedRoleId.HasValue)
                    {
                        continue;
                    }

                    var mutedRole = guild.GetRole(dbGuild.MutedRoleId.Value);

                    if (mutedRole == null || !await mutedRole.CanUseAsync())
                    {
                        continue;
                    }

                    var mutes = await ffaContext.Mutes.ToListAsync();

                    foreach (var mute in mutes)
                    {
                        if (mute.EndsAt.Subtract(DateTime.UtcNow).Ticks <= 0)
                        {
                            await ffaContext.RemoveAsync<Mute>(mute.Id);

                            var guildUser = await guild.GetUserAsync(mute.UserId);

                            if (guildUser != null)
                            {
                                await guildUser.RemoveRoleAsync(mutedRole);
                                await _moderationService.LogAutoUnmuteAsync(ffaContext, guild, guildUser);
                            }
                        }
                    }
                }
            });
    }
}
