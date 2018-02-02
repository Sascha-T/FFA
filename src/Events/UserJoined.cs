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
        private readonly Credentials _credentials;

        public UserJoined(DiscordSocketClient client, FFAContext ffaContext, Credentials credentials)
        {
            _client = client;
            _ffaContext = ffaContext;
            _credentials = credentials;

            _client.UserJoined += OnUserJoinedAsync;
        }

        private async Task OnUserJoinedAsync(SocketGuildUser guildUser)
        {
            if (await _ffaContext.Mutes.AnyAsync((x) => x.UserId == guildUser.Id))
            {
                var mutedRole = guildUser.Guild.GetRole(_credentials.MutedRoleId);

                await guildUser.AddRoleAsync(mutedRole);
            }
        }
    }
}
