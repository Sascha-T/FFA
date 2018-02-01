using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.Linq;

namespace FFA.Events
{
    public class ChannelCreated
    {
        private readonly DiscordSocketClient _client;
        private readonly OverwritePermissions _mutedOverwritePermissions;

        public ChannelCreated(DiscordSocketClient client)
        {
            _client = client;
            _mutedOverwritePermissions = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Deny);

            _client.ChannelCreated += OnChannelCreatedAsync;
        }

        private async Task OnChannelCreatedAsync(SocketChannel channel)
        {
            if (channel is ITextChannel textChannel)
            {
                var muted = textChannel.Guild.Roles.FirstOrDefault((x) => x.Name == "Muted");

                if (muted != default(IRole))
                {
                    await textChannel.AddPermissionOverwriteAsync(muted, _mutedOverwritePermissions);
                }
            }
        }
    }
}
