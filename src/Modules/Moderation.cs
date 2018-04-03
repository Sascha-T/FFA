using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Database.Models;
using FFA.Extensions.Discord;
using FFA.Preconditions.Command;
using FFA.Preconditions.Parameter;
using FFA.Services;
using FFA.Utility;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Modules
{
    [Name("Moderation")]
    [Summary("Commands reserved for the most reputable users to moderate the guild.")]
    [GuildOnly]
    [NotMuted]
    [Top(Config.TOP_MOD)]
    public sealed class Moderation : ModuleBase<Context>
    {
        private readonly ModerationService _modService;
        private readonly IMongoCollection<Mute> _dbMutes;

        public Moderation(ModerationService moderationService, IMongoCollection<Mute> dbMutes)
        {
            _modService = moderationService;
            _dbMutes = dbMutes;
        }

        [Command("Mute")]
        [Summary("Mute any guild user.")]
        [SetMutedRole]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task MuteAsync(
            [Summary("Jimbo#5555")] [NoSelf] [HigherReputation] [NotMutedParam] [UserOverride] IUser user,
            [Summary("2c")] Rule rule,
            [Summary("8h")] [MinimumHours(Config.MIN_MUTE_LENGTH)] TimeSpan length,
            [Summary("stop with all that ruckus!")] [Remainder] [MaximumLength(Config.MAX_REASON_LENGTH)] string reason = null)
        {
            var guildUser = await Context.Guild.GetUserAsync(user.Id);

            if (rule.MaxMuteLength.HasValue && length > rule.MaxMuteLength)
            {
                await Context.ReplyErrorAsync($"The maximum mute length of this rule is {rule.MaxMuteLength.Value.TotalHours}h.");
            }
            else
            {
                await guildUser?.AddRoleAsync(Context.Guild.GetRole(Context.DbGuild.MutedRoleId.Value));
                await Context.ReplyAsync($"You have successfully muted {user.Bold()}.");
                await _modService.CreateMute(Context, user, rule, length, reason);
            }
        }
        
        [Command("Unmute")]
        [Summary("Unmute any guild user.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task UnmuteAsync(
            [Summary("Billy#6969")] [NoSelf] [Muted] IGuildUser guildUser,
            [Summary("you best stop flirting with Mrs Ruckus")] [Remainder]
            [MaximumLength(Config.MAX_REASON_LENGTH)] string reason)
        {
            await guildUser.RemoveRoleAsync(Context.Guild.GetRole(Context.DbGuild.MutedRoleId.Value));
            await Context.ReplyAsync($"You have successfully unmuted {guildUser.Bold()}.");
            await _modService.RemoveMute(Context, guildUser, reason);
        }

        [Command("Clear")]
        [Alias("prune", "purge")]
        [Summary("Delete a specified amount of messages sent by any guild user.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Clear(
            [Summary("SteveJr#3333")] [NoSelf] [HigherReputation] [UserOverride] IUser user,
            [Summary("3a")] Rule rule,
            [Summary("20")] [Between(Config.MIN_CLEAR, Config.MAX_CLEAR)] int quantity = Config.CLEAR_DEFAULT,
            [Summary("stop spamming")] [Remainder] [MaximumLength(Config.MAX_REASON_LENGTH)] string reason = null)
        {
            var messages = await Context.Channel.GetMessagesAsync().FlattenAsync();
            var filtered = messages.Where(x => x.Author.Id == user.Id).Take(quantity);

            await Context.TextChannel.DeleteMessagesAsync(filtered);

            var msg = await Context.ReplyAsync($"You have successfully deleted {quantity} messages sent by {user.Bold()}.");

            await Task.Delay(Config.CLEAR_DELETE_DELAY);
            await msg.DeleteAsync();
            await _modService.LogClearAsync(Context, user, rule, quantity, reason);
        }
    }
}
