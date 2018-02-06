using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Extensions;
using FFA.Preconditions;
using System.Threading.Tasks;

namespace FFA.Modules
{
    public sealed class Memes : ModuleBase<Context>
    {
        [Command("Bully")]
        [Summary("Change anyone's nickname to whatever you please.")]
        [Top(40)]
        public async Task BullyAsync([Summary("LooneyInBed#0059")]IGuildUser guildUser,
                                     [Summary("tiny fucking cock")] [MaximumLength(Configuration.MAX_NICKNAME_LENGTH)]
                                     [Cooldown(Configuration.BULLY_COOLDOWN)] [Remainder] string nickname)
        {
            await guildUser.ModifyAsync((x) => x.Nickname = nickname);
            await Context.ReplyAsync($"You have successfully bullied {guildUser.Bold()} to `{nickname}`.");
        }
    }
}
