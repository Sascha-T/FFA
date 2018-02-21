using Discord;
using System.Threading.Tasks;

namespace FFA.Extensions
{
    public static class ITextChannelExtensions
    {
        public static async Task<bool> CanSendAsync(this ITextChannel channel)
        {
            var currentUser = await channel.Guild.GetCurrentUserAsync();

            return currentUser.GetPermissions(channel).SendMessages;
        }
    }
}
