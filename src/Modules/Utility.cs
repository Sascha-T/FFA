using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Extensions.Discord;
using FFA.Extensions.System;
using FFA.Preconditions.Parameter;
using FFA.Services;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFA.Modules
{
    [Name("Utility")]
    [Summary("General utility commands.")]
    public sealed class Utility : ModuleBase<Context>
    {
        private readonly DeletedMessagesService _deletedMsgsService;

        public Utility(DeletedMessagesService deletedMsgsService)
        {
            _deletedMsgsService = deletedMsgsService;
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
            var elems = new string[deletedMsgs.Count * 2];

            for (int i = 0, j = 0; i < deletedMsgs.Count; i++)
            {
                elems[j++] = deletedMsgs[i].Author.Bold();
                elems[j++] = deletedMsgs[i].Content;
            }

            if (elems.Length > 0)
                return Context.SendFieldsAsync(fieldOrValue: elems);
            else
                return Context.ReplyErrorAsync("There have been no recent deleted messages in this channel.");
        }
    }
}
