using FFA.Common;
using FFA.Database.Models;
using FFA.Entities.FFATimer;
using FFA.Extensions.Database;
using FFA.Extensions.Discord;
using FFA.Services;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace FFA.Timers
{
    public sealed class AutoUnmute : FFATimer
    {
        private readonly IMongoCollection<Guild> _dbGuilds;
        private readonly IMongoCollection<Mute> _dbMutes;
        private readonly ModerationService _moderationService;

        public AutoUnmute(IServiceProvider provider) : base(provider, Config.AUTO_UNMUTE_TIMER)
        {
            _dbGuilds = provider.GetRequiredService<IMongoCollection<Guild>>();
            _dbMutes = provider.GetRequiredService<IMongoCollection<Mute>>();
            _moderationService = provider.GetRequiredService<ModerationService>();
        }

        protected override async Task Execute()
        {
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
                    if (mute.Timestamp.Subtract(mute.Length).CompareTo(DateTimeOffset.UtcNow) < 0)
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
    }
}
