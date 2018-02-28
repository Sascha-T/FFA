using Discord;
using FFA.Common;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Extensions.Discord
{
    public static class IGuildExtensions
    {
        public static async Task<IRole> GetOrCreateRoleAsync(this IGuild guild, string name, Color color)
        {
            var role = guild.Roles.FirstOrDefault(x => x.Name == name);

            if (role == default(IRole))
            {
                if (guild.Roles.Count == Configuration.MAX_ROLES)
                    await guild.Roles.First(x => x.Name.StartsWith('#')).DeleteAsync();

                role = await guild.CreateRoleAsync(name, color: color);
            }

            return role;
        }
    }
}
