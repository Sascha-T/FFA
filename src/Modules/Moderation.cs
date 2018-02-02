using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using FFA.Common;
using FFA.Extensions;
using FFA.Preconditions;
using FFA.Database;
using FFA.Database.Models;
using System;

namespace FFA.Modules
{
    [Name("Moderation")]
    [GuildOnly]
    // [ModOnly]
    public sealed class Moderation : ModuleBase<Context>
    {
        private readonly Credentials _credentials;
        private readonly FFAContext _ffaContext;

        public Moderation(Credentials credentials, FFAContext ffaContext)
        {
            _credentials = credentials;
            _ffaContext = ffaContext;
        }

        [Command("mute")]
        [Summary("Mute any guild member.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task MuteAsync(IGuildUser guildUser, Rule rule, TimeSpan length, [Remainder] string reason = null)
        {
            if (guildUser.RoleIds.Contains(_credentials.MutedRoleId))
            {
                await Context.ReplyErrorAsync("This user is already muted.");
            }
            else
            {
                await guildUser.AddRoleAsync(Context.Guild.Roles.Single((x) => x.Id == _credentials.MutedRoleId));
                await _ffaContext.AddAsync(new Mute(guildUser.Id, DateTime.UtcNow.Add(length)));
                await Context.ReplyAsync("You have successfully muted " + guildUser.Tag() + ".");
            }
        }

        [Command("unmute")]
        [Summary("Unmute any guild member.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task UnmuteAsync(IGuildUser guildUser, [Remainder] string reason = null)
        {
            if (!guildUser.RoleIds.Contains(_credentials.MutedRoleId))
            {
                await Context.ReplyErrorAsync("This user is not muted.");
            }
            else
            {
                await _ffaContext.RemoveAsync<Mute>((x) => x.UserId == guildUser.Id);
                await guildUser.RemoveRoleAsync(Context.Guild.Roles.Single((x) => x.Id == _credentials.MutedRoleId));
                await Context.ReplyAsync("You have successfully unmuted " + guildUser.Tag() + ".");
            }
        }
    }
}
