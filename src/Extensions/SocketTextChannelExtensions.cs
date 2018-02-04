using Discord;
using System.Threading.Tasks;

namespace FFA.Extensions
{
    public static class SocketTextChannelExtensions
    {
        public static async Task<bool> CanSend(this ITextChannel channel)
        {
            var currentUser = await channel.Guild.GetCurrentUserAsync();
            var permissionOverwrites = currentUser.GetPermissions(channel);

            return permissionOverwrites.SendMessages;
        }
    }
}
