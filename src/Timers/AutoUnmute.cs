using Discord;
using Discord.WebSocket;
using FFA.Common;
using FFA.Database;
using FFA.Database.Models;
using FFA.Extensions;
using FFA.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FFA.Timers
{
    public class AutoUnmute
    {
        private readonly IDiscordClient _client;
        private readonly FFAContext _ffaContext;
        private readonly ModerationService _moderationService;
        private readonly Timer _timer;
        private readonly AutoResetEvent _autoEvent;

        public AutoUnmute(DiscordSocketClient client, FFAContext ffaContext, ModerationService moderationService)
        {
            _client = client;
            _ffaContext = ffaContext;
            _moderationService = moderationService;
            _autoEvent = new AutoResetEvent(false);
            _timer = new Timer(Execute, _autoEvent, 0, Configuration.AUTO_UNMUTE_TIMER);
        }

        private void Execute(object state)
            => Task.Run(async () =>
            {
                foreach (var guild in await _client.GetGuildsAsync())
                {
                    var dbGuild = await _ffaContext.GetGuildAsync(guild.Id);

                    if (!dbGuild.MutedRoleId.HasValue)
                    {
                        continue;
                    }

                    var mutedRole = guild.GetRole(dbGuild.MutedRoleId.Value);

                    if (mutedRole == null || !await mutedRole.CanUseRoleAsync())
                    {
                        continue;
                    }

                    var mutes = await _ffaContext.Mutes.ToListAsync();

                    foreach (var mute in mutes)
                    {
                        if (mute.EndsAt.Subtract(DateTime.UtcNow).Ticks <= 0)
                        {
                            await _ffaContext.RemoveAsync<Mute>(mute.Id);

                            var guildUser = await guild.GetUserAsync(mute.UserId);

                            if (guildUser != null)
                            {
                                await guildUser.RemoveRoleAsync(mutedRole);
                                // TODO: specialized log for auto unmute
                                await _moderationService.LogUnmuteAsync(guild, _client.CurrentUser, guildUser);
                            }
                        }
                    }
                }
            });
    }
}
