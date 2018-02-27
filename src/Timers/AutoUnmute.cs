using Discord;
using Discord.WebSocket;
using FFA.Common;
using FFA.Database.Models;
using FFA.Extensions.Database;
using FFA.Extensions.Discord;
using FFA.Services;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FFA.Timers
{
    internal sealed class AutoUnmute
    {
        private readonly IServiceProvider _provider;
        private readonly IDiscordClient _client;
        private readonly LoggingService _logger;
        private readonly ModerationService _moderationService;
        private readonly Timer _timer;
        private readonly AutoResetEvent _autoEvent;

        internal AutoUnmute(IServiceProvider provider)
        {
            _provider = provider;
            _logger = _provider.GetRequiredService<LoggingService>();
            _client = _provider.GetRequiredService<DiscordSocketClient>();
            _moderationService = _provider.GetRequiredService<ModerationService>();
            _autoEvent = new AutoResetEvent(false);
            _timer = new Timer(Execute, _autoEvent, 0, Configuration.AUTO_UNMUTE_TIMER);
        }

        private void Execute(object state)
            => Task.Run(async () =>
            {
                try
                {
                    var guildCollection = _provider.GetRequiredService<IMongoCollection<Guild>>();
                    var muteCollection = _provider.GetRequiredService<IMongoCollection<Mute>>();

                    foreach (var guild in await _client.GetGuildsAsync())
                    {
                        var dbGuild = await guildCollection.GetGuildAsync(guild.Id);

                        if (!dbGuild.MutedRoleId.HasValue)
                            continue;

                        var mutedRole = guild.GetRole(dbGuild.MutedRoleId.Value);

                        if (mutedRole == null || !await mutedRole.CanUseAsync())
                            continue;

                        var mutes = await muteCollection.FindAsync(FilterDefinition<Mute>.Empty);

                        foreach (var mute in mutes.ToEnumerable())
                        {
                            if (mute.EndsAt - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() <= 0)
                            {
                                await muteCollection.DeleteOneAsync(x => x.Id == mute.Id);

                                var guildUser = await guild.GetUserAsync(mute.UserId);

                                if (guildUser != null)
                                {
                                    await guildUser.RemoveRoleAsync(mutedRole);
                                    await _moderationService.LogAutoUnmuteAsync(guild, guildUser);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await _logger.LogAsync(LogSeverity.Error, ex.ToString());
                }
            });
    }
}
