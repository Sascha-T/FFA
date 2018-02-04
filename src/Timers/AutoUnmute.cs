using Discord.WebSocket;
using FFA.Database;
using FFA.Database.Models;
using FFA.Extensions;
using FFA.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;

namespace FFA.Timers
{
    public class AutoUnmute
    {
        private readonly DiscordSocketClient _client;
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
            // TODO: put auto unnut length in configuration
            _timer = new Timer(Execute, _autoEvent, 0, 60000);
        }

        private async void Execute(object state)
        {
            foreach (var guild in _client.Guilds)
            {
                var dbGuild = await _ffaContext.GetGuildAsync(guild.Id);

                if (!dbGuild.MutedRoleId.HasValue)
                {
                    continue;
                }

                var mutedRole = guild.GetRole(dbGuild.MutedRoleId.Value);

                if (mutedRole == null || !mutedRole.CanUseRole())
                {
                    continue;
                }

                var mutes = await _ffaContext.Mutes.ToListAsync();

                foreach (var mute in mutes)
                {
                    if (mute.EndsAt.Subtract(DateTime.UtcNow).Ticks <= 0)
                    {
                        await _ffaContext.RemoveAsync<Mute>(mute.Id);

                        var guildUser = guild.GetUser(mute.UserId);

                        if (guildUser != null)
                        {
                            await guildUser.RemoveRoleAsync(mutedRole);
                            // TODO: specialized log for auto unmute
                            await _moderationService.LogUnmute(guild, _client.CurrentUser, guildUser);
                        }
                    }
                }
            }
        }
    }
}
