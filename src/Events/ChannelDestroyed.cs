using System;
using System.Threading.Tasks;
using Discord;
using FFA.Database.Models;
using FFA.Entities.Event;
using FFA.Extensions.Database;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace FFA.Events
{
    public sealed class ChannelDestroyed : Event
    {
        private readonly IMongoCollection<Guild> _dbGuilds;

        public ChannelDestroyed(IServiceProvider provider) : base(provider)
        {
            _dbGuilds = provider.GetRequiredService<IMongoCollection<Guild>>();

            _client.ChannelDestroyed += OnChannelDestroyed;
        }

        public Task OnChannelDestroyed(IChannel channel)
            => _taskService.TryRun(async () =>
            {
                var textChannel = channel as ITextChannel;

                if (textChannel == null)
                    return;

                var dbGuild = await _dbGuilds.GetGuildAsync(textChannel.Guild.Id);

                if (channel.Id == dbGuild.LogChannelId)
                    await _dbGuilds.UpdateAsync(dbGuild, x => x.LogChannelId = null);
                else if (channel.Id == dbGuild.RulesChannelId)
                    await _dbGuilds.UpdateAsync(dbGuild, x => x.RulesChannelId = null);
                else if (channel.Id == dbGuild.ArchiveChannelId)
                    await _dbGuilds.UpdateAsync(dbGuild, x => x.ArchiveChannelId = null);
            });
    }
}
