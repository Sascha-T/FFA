using Discord.WebSocket;
using FFA.Common;
using FFA.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FFA.Events
{
    public sealed class UserJoined
    {
        private readonly DiscordSocketClient _client;
        private readonly FFAContext _ffaContext;

        public UserJoined(DiscordSocketClient client, FFAContext ffaContext)
        {
            _client = client;
            _ffaContext = ffaContext;

            _client.UserJoined += OnUserJoinedAsync;
        }

        private async Task OnUserJoinedAsync(SocketGuildUser guildUser)
        {
            var dbGuild = await _ffaContext.GetGuildAsync(guildUser.Guild.Id);

            if (dbGuild.MutedRoleId.HasValue && await _ffaContext.Mutes.AnyAsync(x => x.GuildId == guildUser.Id && x.UserId == guildUser.Id))
            {
                var mutedRole = guildUser.Guild.GetRole(dbGuild.MutedRoleId.Value);

                await guildUser.AddRoleAsync(mutedRole);
            }
        }
    }
}
