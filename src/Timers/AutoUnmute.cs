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
    public sealed class AutoUnmute
    {
        private readonly IMongoCollection<Guild> _dbGuilds;
        private readonly IMongoCollection<Mute> _dbMutes;
        private readonly DiscordSocketClient _client;
        private readonly LoggingService _logger;
        private readonly ModerationService _moderationService;
        private readonly Timer _timer;
        private readonly AutoResetEvent _autoEvent;

        public AutoUnmute(IServiceProvider provider)
        {
            _dbGuilds = provider.GetRequiredService<IMongoCollection<Guild>>();
            _dbMutes = provider.GetRequiredService<IMongoCollection<Mute>>();
            _logger = provider.GetRequiredService<LoggingService>();
            _client = provider.GetRequiredService<DiscordSocketClient>();
            _moderationService = provider.GetRequiredService<ModerationService>();
            _autoEvent = new AutoResetEvent(false);
            _timer = new Timer(Execute, _autoEvent, 0, Config.AUTO_UNMUTE_TIMER);
        }

        private void Execute(object state)
            => Task.Run(async () =>
            {
                try
                {
                    if (_client.ConnectionState != ConnectionState.Connected)
                        return;

                    foreach (var guild in _client.Guilds)
                    {
                        var dbGuild = await _dbGuilds.GetGuildAsync(guild.Id);

                        if (!dbGuild.MutedRoleId.HasValue)
                            continue;

                        var mutedRole = guild.GetRole(dbGuild.MutedRoleId.Value);

                        if (mutedRole == null || !await mutedRole.CanUseAsync())
                            continue;

                        var mutes = await _dbMutes.FindAsync(FilterDefinition<Mute>.Empty);

                        foreach (var mute in mutes.ToEnumerable())
                        {
                            if (mute.Timestamp.Subtract(mute.Length).CompareTo(DateTimeOffset.UtcNow) == 0)
                                continue;

                            await _dbMutes.DeleteOneAsync(x => x.Id == mute.Id);

                            var guildUser = guild.GetUser(mute.UserId);

                            if (guildUser == null)
                                continue;

                            await guildUser.RemoveRoleAsync(mutedRole);
                            await _moderationService.LogAutoUnmuteAsync(guild, guildUser);
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
