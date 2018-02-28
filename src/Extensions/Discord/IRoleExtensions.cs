using Discord;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Extensions.Discord
{
    public static class IRoleExtensions
    {
        public static async Task<bool> CanUseAsync(this IRole role)
        {
            var currentUser = await role.Guild.GetCurrentUserAsync();
            var highestPosition = currentUser.GetRoles().OrderByDescending(x => x.Position).First().Position;

            return currentUser.GuildPermissions.ManageRoles && role.Position < highestPosition;
        }
    }
}