using Discord;
using Discord.WebSocket;
using FFA.Database;
using FFA.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FFA.Events
{
    internal sealed class UserJoined
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;

        internal UserJoined(IServiceProvider provider)
        {
            _provider = provider;
            _client = _provider.GetRequiredService<DiscordSocketClient>();

            _client.UserJoined += OnUserJoinedAsync;
        }

        private Task OnUserJoinedAsync(IGuildUser guildUser)
        {
            Task.Run(async () =>
            {
                var ffaContext = _provider.GetRequiredService<FFAContext>();
                var dbGuild = await ffaContext.GetGuildAsync(guildUser.Guild.Id);

                if (dbGuild.MutedRoleId.HasValue && await ffaContext.Mutes.AnyAsync(x => x.GuildId == guildUser.Guild.Id && x.UserId == guildUser.Id))
                {
                    var mutedRole = guildUser.Guild.GetRole(dbGuild.MutedRoleId.Value);

                    if (mutedRole == null || !await mutedRole.CanUseRoleAsync())
                    {
                        return;
                    }

                    await guildUser.AddRoleAsync(mutedRole);
                }
            });

            return Task.CompletedTask;
        }
    }
}
