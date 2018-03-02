using Discord;
using System.Collections.Generic;
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

        public static async Task<IEnumerable<IGuildUser>> GetMembersAsync(this IRole role)
        {
            var guildUsers = await role.Guild.GetUsersAsync();
            return guildUsers.Where(x => x.RoleIds.Any(y => y == role.Id));
        }
    }
}
