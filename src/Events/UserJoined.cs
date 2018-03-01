using Discord;
using Discord.WebSocket;
using FFA.Database.Models;
using FFA.Extensions.Database;
using FFA.Extensions.Discord;
using FFA.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace FFA.Events
{
    public sealed class UserJoined
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly LoggingService _logger;
        private readonly IMongoCollection<Guild> _dbGuilds;
        private readonly IMongoCollection<Mute> _dbMutes;

        public UserJoined(IServiceProvider provider)
        {
            _provider = provider;
            _client = _provider.GetRequiredService<DiscordSocketClient>();
            _logger = _provider.GetRequiredService<LoggingService>();
            _dbGuilds = _provider.GetRequiredService<IMongoCollection<Guild>>();
            _dbMutes = _provider.GetRequiredService<IMongoCollection<Mute>>();

            _client.UserJoined += OnUserJoinedAsync;
        }

        private Task OnUserJoinedAsync(IGuildUser guildUser)
        {
            Task.Run(async () =>
            {
                try
                {
                    var dbGuild = await _dbGuilds.GetGuildAsync(guildUser.Guild.Id);

                    if (!dbGuild.MutedRoleId.HasValue || !await _dbMutes.AnyAsync(x => x.GuildId == guildUser.Guild.Id && x.UserId == guildUser.Id))
                        return;

                    var mutedRole = guildUser.Guild.GetRole(dbGuild.MutedRoleId.Value);

                    if (mutedRole == null || !await mutedRole.CanUseAsync())
                        return;

                    await guildUser.AddRoleAsync(mutedRole);
                }
                catch (Exception ex)
                {
                    await _logger.LogAsync(LogSeverity.Error, ex.ToString());
                }
            });

            return Task.CompletedTask;
        }
    }
}
