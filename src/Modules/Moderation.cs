using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using FFA.Common;
using FFA.Extensions;
using FFA.Preconditions;

namespace FFA.Modules
{
    [Name("Moderation")]
    [GuildOnly]
    // [ModOnly]
    public sealed class Moderation : ModuleBase<Context>
    {
        private readonly Credentials _credentials;

        public Moderation(Credentials credentials)
        {
            _credentials = credentials;
        }

        [Command("mute")]
        [Summary("Mute any guild member.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Mute(IGuildUser guildUser, [Remainder] string reason = null)
        {
            var mutedRole = Context.Guild.Roles.Single((x) => x.Id == _credentials.MutedRoleId);

            if (guildUser.RoleIds.Contains(mutedRole.Id))
            {
                await Context.ReplyAsync("This user is already muted.");
            }
            else
            {
                await guildUser.AddRoleAsync(mutedRole);
                await Context.ReplyAsync("You have successfully muted " + guildUser.Tag() + ".");
            }
        }

        [Command("unmute")]
        [Summary("Unmute any guild member.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Unmute(IGuildUser guildUser, [Remainder] string reason = null)
        {
            var mutedRole = Context.Guild.Roles.Single((x) => x.Id == _credentials.MutedRoleId);

            if (!guildUser.RoleIds.Contains(mutedRole.Id))
            {
                await Context.ReplyAsync("This user is not muted.");
            }
            else
            {
                await guildUser.RemoveRoleAsync(mutedRole);
                await Context.ReplyAsync("You have successfully unmuted " + guildUser.Tag() + ".");
            }
        }
    }
}
