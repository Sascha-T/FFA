using Discord;
using System.Threading.Tasks;

namespace FFA.Extensions
{
    public static class ITextChannelExtensions
    {
        public static async Task<bool> CanSendAsync(this ITextChannel channel)
        {
            var currentUser = await channel.Guild.GetCurrentUserAsync();
            var permissionOverwrites = currentUser.GetPermissions(channel);

            return permissionOverwrites.SendMessages;
        }
    }
}
