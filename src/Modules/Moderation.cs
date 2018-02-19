using Discord;
using Discord.Commands;
using FFA.Common;
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
    [Top(Configuration.TOP_MOD)]
    public sealed class Moderation : ModuleBase<Context>
    {
        private readonly ModerationService _moderationService;

        public Moderation(ModerationService moderationService)
        {
            _moderationService = moderationService;
        }

        [Command("Mute")]
        [Summary("Mute any guild user.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task MuteAsync([Summary("Jimbo#5555")] [NoSelf] [HigherReputation] IGuildUser guildUser,
                                    [Summary("2c")] Rule rule,
                                    [Summary("8h")] [MinimumHours(Configuration.MIN_MUTE_LENGTH)] TimeSpan length,
                                    [Summary("stop with all that ruckus!")] [Remainder] string reason = null)
        {
            if (!Context.DbGuild.MutedRoleId.HasValue)
            {
                await Context.ReplyErrorAsync("The muted role has not been set.");
            }
            else if (rule.MaxMuteLength.HasValue && length > rule.MaxMuteLength)
            {
                await Context.ReplyErrorAsync($"The maximum mute length of this rule is {rule.MaxMuteLength.Value.TotalHours}h.");
            }
            else if (guildUser.RoleIds.Contains(Context.DbGuild.MutedRoleId.Value))
            {
                await Context.ReplyErrorAsync("This user is already muted.");
            }
            else
            {
                await guildUser.AddRoleAsync(Context.Guild.GetRole(Context.DbGuild.MutedRoleId.Value));
                await Context.Db.AddAsync(new Mute(Context.Guild.Id, guildUser.Id, DateTimeOffset.UtcNow.Add(length)));
                await Context.ReplyAsync($"You have successfully muted {guildUser.Bold()}.");
                await _moderationService.LogMuteAsync(Context, guildUser, rule, length, reason);
            }
        }

        [Command("Unmute")]
        [Summary("Unmute any guild user.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task UnmuteAsync([Summary("Billy#6969")] [NoSelf] [Cooldown(Configuration.UNMUTE_COOLDOWN)] IGuildUser guildUser,
                                      [Summary("you best stop flirting with Mrs Ruckus")] [Remainder] string reason = null)
        {
            if (!Context.DbGuild.MutedRoleId.HasValue)
            {
                await Context.ReplyErrorAsync("The muted role has not been set.");
            }
            else if (!guildUser.RoleIds.Contains(Context.DbGuild.MutedRoleId.Value))
            {
                await Context.ReplyErrorAsync("This user is not muted.");
            }
            else
            {
                await Context.Db.RemoveAsync<Mute>(x => x.UserId == guildUser.Id);
                await guildUser.RemoveRoleAsync(Context.Guild.GetRole(Context.DbGuild.MutedRoleId.Value));
                await Context.ReplyAsync($"You have successfully unmuted {guildUser.Bold()}.");
                await _moderationService.LogUnmuteAsync(Context, guildUser, reason);
            }
        }

        [Command("Clear")]
        [Alias("prune", "purge")]
        [Summary("Delete a specified amount of messages sent by any guild user.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Clear([Summary("SteveJr#3333")] [NoSelf] IUser user,
                                [Summary("3a")] Rule rule,
                                [Summary("20")] [Between(Configuration.MIN_CLEAR, Configuration.MAX_CLEAR)] int quantity = Configuration.CLEAR_DEFAULT,
                                [Summary("that's enough pornos for tonight Steve")] [Remainder] string reason = null)
        {
            var messages = await Context.Channel.GetMessagesAsync().FlattenAsync();
            var filtered = messages.Where(x => x.Author.Id == user.Id).Take(quantity);

            await Context.TextChannel.DeleteMessagesAsync(filtered);

            var msg = await Context.ReplyAsync($"You have successfully deleted {quantity} messages sent by {user.Bold()}.");

            await Task.Delay(Configuration.CLEAR_DELETE_DELAY);
            await msg.DeleteAsync();
            await _moderationService.LogClearAsync(Context, user, rule, quantity, reason);
        }
    }
}
