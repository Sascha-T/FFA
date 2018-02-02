using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Database;
using FFA.Database.Models;
using FFA.Extensions;
using FFA.Preconditions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Modules
{
    [Name("Moderation")]
    [GuildOnly]
    [TopTwenty]
    public sealed class Moderation : ModuleBase<Context>
    {
        private readonly Credentials _credentials;
        private readonly FFAContext _ffaContext;

        public Moderation(Credentials credentials, FFAContext ffaContext)
        {
            _credentials = credentials;
            _ffaContext = ffaContext;
        }

        // TODO: be able to mute IUser
        [Command("mute")]
        [Summary("Mute any guild user.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task MuteAsync([Summary("Jimbo#5555")] IGuildUser guildUser, 
                                    [Summary("2c")] Rule rule, 
                                    [Summary("24h")] TimeSpan length, 
                                    [Summary("stop with all that ruckus!")] [Remainder] string reason = null)
        {
            if (guildUser.RoleIds.Contains(_credentials.MutedRoleId))
            {
                await Context.ReplyErrorAsync("This user is already muted.");
            }
            else
            {
                await guildUser.AddRoleAsync(Context.Guild.GetRole(_credentials.MutedRoleId));
                await _ffaContext.AddAsync(new Mute(guildUser.Id, DateTime.UtcNow.Add(length)));
                await Context.ReplyAsync("You have successfully muted " + guildUser.Tag() + ".");
            }
        }

        [Command("unmute")]
        [Summary("Unmute any guild user.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task UnmuteAsync([Summary("Billy#6969")] IGuildUser guildUser, 
                                      [Summary("you best stop flirting with Mrs Ruckus")] [Remainder] string reason = null)
        {
            if (!guildUser.RoleIds.Contains(_credentials.MutedRoleId))
            {
                await Context.ReplyErrorAsync("This user is not muted.");
            }
            else
            {
                await _ffaContext.RemoveAsync<Mute>((x) => x.UserId == guildUser.Id);
                await guildUser.RemoveRoleAsync(Context.Guild.GetRole(_credentials.MutedRoleId));
                await Context.ReplyAsync("You have successfully unmuted " + guildUser.Tag() + ".");
            }
        }
    }
}
