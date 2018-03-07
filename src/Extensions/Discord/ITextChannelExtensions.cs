using Discord;
using System.Threading.Tasks;

namespace FFA.Extensions.Discord
{
    public static class ITextChannelExtensions
    {
        // TODO: fix method, when not admin = returns false
        public static async Task<bool> CanSendAsync(this ITextChannel channel)
        {
            var currentUser = await channel.Guild.GetCurrentUserAsync();

            return currentUser.GetPermissions(channel).SendMessages;
        }
    }
}
