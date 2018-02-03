using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Database;
using FFA.Database.Models;
using FFA.Extensions;
using FFA.Preconditions;
using FFA.Services;
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
        private readonly FFAContext _ffaContext;
        private readonly ModerationService _moderationService;

        public Moderation(FFAContext ffaContext, ModerationService moderationService)
        {
            _ffaContext = ffaContext;
            _moderationService = moderationService;
        }
        
        [Command("Mute")]
        [Summary("Mute any guild user.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task MuteAsync([Summary("Jimbo#5555")] IGuildUser guildUser, 
                                    [Summary("2c")] Rule rule, 
                                    [Summary("8h")] TimeSpan length, 
                                    [Summary("stop with all that ruckus!")] [Remainder] string reason = null)
        {
            var dbGuild = await _ffaContext.GetGuildAsync(Context.Guild.Id);

            if (!dbGuild.MutedRoleId.HasValue)
            {
                await Context.ReplyErrorAsync("The muted role has not been set.");
            }
            else if (rule.MaxMuteLength.HasValue && length > rule.MaxMuteLength)
            {
                await Context.ReplyErrorAsync($"The maximum mute length of this rule is {rule.MaxMuteLength.Value.TotalHours}h.");
            }
            else if (guildUser.RoleIds.Contains(dbGuild.MutedRoleId.Value))
            {
                await Context.ReplyErrorAsync("This user is already muted.");
            }
            else
            {
                await guildUser.AddRoleAsync(Context.Guild.GetRole(dbGuild.MutedRoleId.Value));
                await _ffaContext.AddAsync(new Mute(Context.Guild.Id, guildUser.Id, DateTime.UtcNow.Add(length)));
                await Context.ReplyAsync($"You have successfully muted {guildUser.Tag()}.");
                await _moderationService.LogMute(Context.Guild, Context.User, guildUser, rule, length, reason);
            }
        }

        [Command("Unmute")]
        [Summary("Unmute any guild user.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task UnmuteAsync([Summary("Billy#6969")] IGuildUser guildUser, 
                                      [Summary("you best stop flirting with Mrs Ruckus")] [Remainder] string reason = null)
        {
            var dbGuild = await _ffaContext.GetGuildAsync(Context.Guild.Id);

            if (!dbGuild.MutedRoleId.HasValue)
            {
                await Context.ReplyErrorAsync("The muted role has not been set.");
            }
            else if (!guildUser.RoleIds.Contains(dbGuild.MutedRoleId.Value))
            {
                await Context.ReplyErrorAsync("This user is not muted.");
            }
            else
            {
                await _ffaContext.RemoveAsync<Mute>(x => x.UserId == guildUser.Id);
                await guildUser.RemoveRoleAsync(Context.Guild.GetRole(dbGuild.MutedRoleId.Value));
                await Context.ReplyAsync($"You have successfully unmuted {guildUser.Tag()}.");
                await _moderationService.LogUnmute(Context.Guild, Context.User, guildUser, reason);
            }
        }
    }
}
