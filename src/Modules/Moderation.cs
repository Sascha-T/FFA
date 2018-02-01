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
        public async Task Mute(IGuildUser user, [Remainder] string reason = null)
        {
            var mutedRole = Context.Guild.Roles.Single((x) => x.Id == _credentials.MutedRoleId);

            if (user.RoleIds.Contains(mutedRole.Id))
            {
                await Context.ReplyAsync("This user is already muted.");
            }
            else
            {
                await user.AddRoleAsync(mutedRole);
                await Context.ReplyAsync("You have successfully muted " + user.Tag() + ".");
            }
        }

        [Command("unmute")]
        [Summary("Unmute any guild member.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Unmute(IGuildUser user, [Remainder] string reason = null)
        {
            var mutedRole = Context.Guild.Roles.Single((x) => x.Id == _credentials.MutedRoleId);

            if (!user.RoleIds.Contains(mutedRole.Id))
            {
                await Context.ReplyAsync("This user is not muted.");
            }
            else
            {
                await user.RemoveRoleAsync(mutedRole);
                await Context.ReplyAsync("You have successfully unmuted " + user.Tag() + ".");
            }
        }
    }
}
