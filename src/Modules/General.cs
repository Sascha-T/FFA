using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Extensions;
using FFA.Preconditions;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Modules
{
    public sealed class General : ModuleBase<Context>
    {
        [Command("Bully")]
        [Summary("Change anyone's nickname to whatever you please.")]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [Top(Configuration.TOP_COLOR)]
        public async Task BullyAsync([Summary("LooneyInBed#0059")] IGuildUser guildUser,
                                     [Summary("tiny fucking cock")] [MaximumLength(Configuration.MAX_NICKNAME_LENGTH)]
                                     [Cooldown(Configuration.BULLY_COOLDOWN)] [Remainder] string nickname)
        {
            await guildUser.ModifyAsync((x) => x.Nickname = nickname);
            await Context.ReplyAsync($"You have successfully bullied {guildUser.Bold()} to `{nickname}`.");
        }

        [Command("Color")]
        [Alias("colour")]
        [Summary("Give yourself a role with any color you please.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Top(Configuration.TOP_COLOR)]
        public async Task ColorAsync([Summary("#FF0000")] [Remainder] [Cooldown(Configuration.COLOR_COOLDOWN)] Color color)
        {
            // TODO: move role finding & creation to helper method
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name.StartsWith('#') && x.Color.RawValue == color.RawValue);

            if (role == default(IRole))
            {
                if (Context.Guild.Roles.Count == Configuration.MAX_ROLES)
                {
                    await Context.Guild.Roles.First(x => x.Name.StartsWith('#')).DeleteAsync();
                }

                // TODO: fixed length for role name
                role = await Context.Guild.CreateRoleAsync($"{color}".ToUpper(), color: color);
            }

            var existingRoles = Context.GuildUser.GetRoles().Where(x => x.Name.StartsWith('#'));

            await Context.GuildUser.RemoveRolesAsync(existingRoles);
            await Context.GuildUser.AddRoleAsync(role);
            await Context.ReplyAsync("You have successfully set your role color.");
        }
    }
}
