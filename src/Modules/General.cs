using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Extensions.Discord;
using FFA.Preconditions;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Modules
{
    public sealed class General : ModuleBase<Context>
    {
        [Command("Color")]
        [Alias("colour")]
        [Summary("Give yourself a role with any color you please.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Top(Configuration.TOP_COLOR)]
        public async Task ColorAsync([Summary("#FF0000")] [Remainder] [Cooldown(Configuration.COLOR_COOLDOWN)] Color color)
        {
            var role = await Context.Guild.GetOrCreateRoleAsync(color.GetFormattedString(), color);
            var existingColorRoles = Context.GuildUser.GetRoles().Where(x => x.Name.StartsWith('#'));

            await Context.GuildUser.RemoveRolesAsync(existingColorRoles);
            await Context.GuildUser.AddRoleAsync(role);
            await Context.ReplyAsync("You have successfully set your role color.");
        }
    }
}
