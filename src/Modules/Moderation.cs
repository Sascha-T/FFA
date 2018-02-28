using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Database.Models;
using FFA.Extensions.Discord;
using FFA.Preconditions.Command;
using FFA.Preconditions.Parameter;
using FFA.Services;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Modules
{
    [Name("Moderation")]
    [GuildOnly]
    [NotMuted]
    [Top(Config.TOP_MOD)]
    public sealed class Moderation : ModuleBase<Context>
    {
        private readonly ModerationService _moderationService;
        private readonly IMongoCollection<Mute> _muteCollection;

        public Moderation(ModerationService moderationService, IMongoDatabase db)
        {
            _moderationService = moderationService;
            _muteCollection = db.GetCollection<Mute>("mutes");
        }

        [Command("Mute")]
        [Summary("Mute any guild user.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task MuteAsync([Summary("Jimbo#5555")] [NoSelf] [HigherReputation] IGuildUser guildUser,
            [Summary("2c")] Rule rule,
            [Summary("8h")] [MinimumHours(Config.MIN_MUTE_LENGTH)] TimeSpan length,
            [Summary("stop with all that ruckus!")] [Remainder]
            [MaximumLength(Config.MAX_REASON_LENGTH)] string reason = null)
        {
            // TODO: add inform user!
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
                await _muteCollection.InsertOneAsync(new Mute(Context.Guild.Id, guildUser.Id, length));
                await Context.ReplyAsync($"You have successfully muted {guildUser.Bold()}.");
                await _moderationService.LogMuteAsync(Context, guildUser, rule, length, reason);
            }
        }
        
        [Command("Unmute")]
        [Summary("Unmute any guild user.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task UnmuteAsync([Summary("Billy#6969")] [NoSelf] IGuildUser guildUser,
            [Summary("you best stop flirting with Mrs Ruckus")] [Remainder]
            [MaximumLength(Config.MAX_REASON_LENGTH)] string reason)
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
                await _muteCollection.DeleteManyAsync(x => x.UserId == guildUser.Id && x.GuildId == Context.Guild.Id);
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
            [Summary("20")] [Between(Config.MIN_CLEAR, Config.MAX_CLEAR)] int quantity = Config.CLEAR_DEFAULT,
            [Summary("that's enough pornos for tonight Steve")] [Remainder]
            [MaximumLength(Config.MAX_REASON_LENGTH)] string reason = null)
        {
            var messages = await Context.Channel.GetMessagesAsync().FlattenAsync();
            var filtered = messages.Where(x => x.Author.Id == user.Id).Take(quantity);

            await Context.TextChannel.DeleteMessagesAsync(filtered);

            var msg = await Context.ReplyAsync($"You have successfully deleted {quantity} messages sent by {user.Bold()}.");

            await Task.Delay(Config.CLEAR_DELETE_DELAY);
            await msg.DeleteAsync();
            await _moderationService.LogClearAsync(Context, user, rule, quantity, reason);
        }
    }
}
