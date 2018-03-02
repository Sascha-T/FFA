using Discord;
using FFA.Database.Models;
using FFA.Entities.Event;
using FFA.Extensions.Database;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace FFA.Events
{
    public sealed class RoleDeleted : Event
    {
        private readonly IMongoCollection<Guild> _dbGuilds;

        public RoleDeleted(IServiceProvider provider) : base(provider)
        {
            _dbGuilds = provider.GetRequiredService<IMongoCollection<Guild>>();

            _client.RoleDeleted += OnRoleDeletedAsync;
        }

        public Task OnRoleDeletedAsync(IRole role)
            => _taskService.TryRun(async () =>
            {
                var dbGuild = await _dbGuilds.GetGuildAsync(role.Guild.Id);

                if (role.Id == dbGuild.MutedRoleId)
                    await _dbGuilds.UpdateAsync(dbGuild, x => x.MutedRoleId = null);
            });
    }
}
