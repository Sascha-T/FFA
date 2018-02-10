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
    [Top(Configuration.TOP_MOD)]
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
        public async Task MuteAsync([Summary("Jimbo#5555")] [NoSelf] [HigherReputation] IGuildUser guildUser,
                                    [Summary("2c")] Rule rule,
                                    [Summary("8h")] [MinimumHours(Configuration.MIN_MUTE_LENGTH)] TimeSpan length,
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
                await Context.ReplyAsync($"You have successfully muted {guildUser.Bold()}.");
                await _moderationService.LogMuteAsync(Context, guildUser, rule, length, reason);
            }
        }

        [Command("Unmute")]
        [Summary("Unmute any guild user.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task UnmuteAsync([Summary("Billy#6969")] [NoSelf] IGuildUser guildUser,
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
                await Context.ReplyAsync($"You have successfully unmuted {guildUser.Bold()}.");
                await _moderationService.LogUnmuteAsync(Context, guildUser, reason);
            }
        }

        [Command("Clear")]
        [Alias("prune", "purge")]
        [Summary("Delete a specified amount of messages sent by any guild user.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Clear([Summary("SteveJr#3333")] [NoSelf] [HigherReputation] IGuildUser guildUser,
                                [Summary("3a")] Rule rule,
                                [Summary("20")] [Between(Configuration.MIN_CLEAR, Configuration.MAX_CLEAR)] int quantity = Configuration.CLEAR_DEFAULT,
                                [Summary("that's enough pornos for tonight Steve")] [Remainder] string reason = null)
        {
            var messages = await Context.Channel.GetMessagesAsync().FlattenAsync();
            var filtered = messages.Where(x => x.Author.Id == guildUser.Id).Take(quantity);

            await Context.TextChannel.DeleteMessagesAsync(filtered);

            var msg = await Context.ReplyAsync($"You have successfully deleted {quantity} messages sent by {guildUser.Bold()}.");

            await Task.Delay(Configuration.CLEAR_DELETE_DELAY);
            await msg.DeleteAsync();
            await _moderationService.LogClearAsync(Context, guildUser, rule, quantity, reason);
        }
    }
}
