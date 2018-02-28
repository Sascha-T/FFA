using Discord;
using System.Collections.Generic;
using System.Linq;

namespace FFA.Extensions.Discord
{
    public static class IGuildUserExtensions
    {
        // TODO: D.NET PR to add guildUser.Roles
        public static IEnumerable<IRole> GetRoles(this IGuildUser guildUser)
            => guildUser.RoleIds.Select(x => guildUser.Guild.GetRole(x)).Where(x => x != null);
    }
}
