using Discord.WebSocket;
using System.Linq;

namespace FFA.Extensions
{
    public static class SocketRoleExtensions
    {
        public static bool CanUseRole(this SocketRole role)
        {
            var highestPosition = role.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position;

            return role.Guild.CurrentUser.GuildPermissions.ManageRoles && role.Position < highestPosition;
        }
    }
}
