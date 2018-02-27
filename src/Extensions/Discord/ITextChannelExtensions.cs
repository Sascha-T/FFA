using Discord;
using System.Threading.Tasks;

namespace FFA.Extensions.Discord
{
    internal static class ITextChannelExtensions
    {
        internal static async Task<bool> CanSendAsync(this ITextChannel channel)
        {
            var currentUser = await channel.Guild.GetCurrentUserAsync();

            return currentUser.GetPermissions(channel).SendMessages;
        }
    }
}
