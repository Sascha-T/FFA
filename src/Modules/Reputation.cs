using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Extensions;
using FFA.Preconditions;
using FFA.Services;
using System.Threading.Tasks;

namespace FFA.Modules
{
    [Name("Reputation")]
    [GuildOnly]
    public sealed class Reputation : ModuleBase<Context>
    {
        private readonly ReputationService _repService;

        public Reputation(ReputationService repService)
        {
            _repService = repService;
        }

        [Command("Mod")]
        [Alias("moderator")]
        [Summary("Informs you whether you are a moderator.")]
        public async Task ModAsync()
        {
            if (await _repService.IsInTopAsync(Context.Db, Configuration.TOP_MOD, Context.User.Id, Context.Guild.Id))
            {
                await Context.ReplyAsync("You are currently a moderator.");
            }
            else
            {
                await Context.ReplyErrorAsync("You are currently not a moderator.");
            }
        }

        [Command("Rep")]
        [Summary("Give reputation to any user.")]
        public async Task RepAsync([Summary("AlabamaTrigger#0001")] [Cooldown(Configuration.REP_COOLDOWN)] [NoSelf] IGuildUser user)
        {
            await Context.Db.UpsertUserAsync(user, x => x.Reputation += Configuration.REP_INCREASE);
            await Context.ReplyAsync($"You have successfully repped {user.Bold()}.");
        }

        [Command("UnRep")]
        [Summary("Remove reputation from any user.")]
        public async Task UnRepAsync([Summary("PapaFag#6666")] [Cooldown(Configuration.UNREP_COOLDOWN)] [NoSelf] IGuildUser user)
        {
            await Context.Db.UpsertUserAsync(user, x => x.Reputation -= Configuration.UNREP_DECREASE);
            await Context.ReplyAsync($"You have successfully unrepped {user.Bold()}.");
        }

        [Command("MyRep")]
        [Summary("Get your current reputation.")]
        public async Task MyRepAsync()
        {
            await Context.DmAsync($"You currently have {Context.DbUser.Reputation} reputation.");
            await Context.ReplyAsync($"You have been DMed with your reputation.");
        }
    }
}
