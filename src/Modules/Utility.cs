using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Database.Models;
using FFA.Extensions.Database;
using FFA.Extensions.Discord;
using FFA.Extensions.System;
using FFA.Preconditions.Parameter;
using FFA.Services;
using FFA.Utility;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Modules
{
    [Name("Utility")]
    [Summary("General utility commands.")]
    public sealed class Utility : ModuleBase<Context>
    {
        private readonly DeletedMessagesService _deletedMsgsService;
        private readonly IMongoCollection<Mute> _dbMutes;

        public Utility(DeletedMessagesService deletedMsgsService, IMongoCollection<Mute> dbMutes)
        {
            _deletedMsgsService = deletedMsgsService;
            _dbMutes = dbMutes;
        }

        [Command("AltCheck")]
        [Alias("alt")]
        [Summary("Verifies whether or not a user is an alterate account.")]
        public Task AltCheck(
            [Summary("Jimbo Steve#8842")] [Remainder] IGuildUser guildUser)
        {
            var joinedAt = guildUser.JoinedAt.GetValueOrDefault();

            return Context.SendAsync(
                $"**Created:** `{guildUser.CreatedAt.ToString("f")}`\n" +
                $"**Joined:** `{joinedAt.ToString("f")}`\n" +
                $"**Difference:** `{joinedAt.Subtract(guildUser.CreatedAt)}`", guildUser.ToString());
        }

        [Command("Deleted")]
        [Alias("deletedmessages", "deletedmsgs")]
        [Summary("Gets the last deleted messages of the channel.")]
        public Task Deleted(
            [Summary("5")] [Between(Config.MIN_DELETED_MSGS, Config.MAX_DELETED_MSGS)] int count = Config.DELETED_MSGS)
        {
            var deletedMsgs = _deletedMsgsService.GetLast(Context.Channel.Id, count);
            var elems = new List<string>();

            for (int i = 0, j = 0; i < deletedMsgs.Count; i++, j += 2)
            {
                var name = deletedMsgs[i].Author.Bold();
                var val = new string(deletedMsgs[i].Content.Take(Config.DELETED_MESSAGES_CHARS).ToArray());

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(val))
                    continue;

                elems.Add(name);
                elems.Add(val);
            }

            return elems.Count > 0 ?
                Context.SendFieldsAsync(fieldOrValue: elems) :
                Context.ReplyErrorAsync("There have been no recent deleted messages in this channel.");
        }


        [Command("TimeLeft")]
        [Alias("left")]
        [Summary("Tell how much time is left on your mute.")]
        public async Task TimeLeftAsync(
            [Summary("hornydevil#0018")] [Remainder] [UserOverride] IUser user = null)
        {
            user = user ?? Context.User;

            var dbMuteUser = await _dbMutes.FindOneAsync(x => x.UserId == user.Id && x.GuildId == Context.Guild.Id &&
                x.Active);

            if (dbMuteUser == null)
            {
                await Context.ReplyErrorAsync($"{(user != Context.User ? user + " isn\'t" : "You aren\'t")} muted.");
            }
            else
            {
                var timeLeft = dbMuteUser.Timestamp.Add(dbMuteUser.Length).Subtract(DateTimeOffset.UtcNow);
                await Context.SendAsync($"**Time left:** {timeLeft.ToString(@"d\.hh\:mm\:ss")}", $"{user}'s Mute");
            }
        }
    }
}
