using Discord;
using FFA.Common;
using FFA.Entities.Service;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class ColorRoleService : Service
    {
        public async Task<IRole> GetOrCreateAsync(IGuild guild, string name, Color color)
        {
            var role = guild.Roles.FirstOrDefault(x => x.Name == name);

            if (role == default(IRole))
            {
                if (guild.Roles.Count == Constants.MAX_ROLES)
                    await guild.Roles.First(x => x.Name.StartsWith('#')).DeleteAsync();

                role = await guild.CreateRoleAsync(name, color: color);
            }

            return role;
        }

        public string FormatColor(Color color)
            => $"#{color.RawValue.ToString($"X{Config.MAX_HEX_LENGTH}")}";
    }
}
