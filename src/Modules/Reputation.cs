using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Database;
using FFA.Extensions;
using FFA.Preconditions;
using System.Threading.Tasks;

namespace FFA.Modules
{
    [Name("Reputation")]
    [GuildOnly]
    public sealed class Reputation : ModuleBase<Context>
    {
        private readonly FFAContext _ffaContext;

        public Reputation(FFAContext ffaContext)
        {
            _ffaContext = ffaContext;
        }

        //[NoSelf]
        [Command("Rep")]
        [Summary("Give reputation to any user.")]
        public async Task RepAsync([Summary("AlabamaTrigger#0001")] [Cooldown(24)] IGuildUser user)
        {
            await _ffaContext.UpsertUserAsync(user, x => x.Reputation++);
            await Context.ReplyAsync($"You have successfully repped {user.Tag()}.");
        }

        [Command("UnRep")]
        [Summary("Remove reputation from any user.")]
        public async Task UnRepAsync([Summary("PapaFag#6666")] [Cooldown(24)] IGuildUser user)
        {
            await _ffaContext.UpsertUserAsync(user, x => x.Reputation--);
            await Context.ReplyAsync($"You have successfully unrepped {user.Tag()}.");
        }

        [Command("MyRep")]
        [Summary("Get your current reputation.")]
        public async Task MyRepAsync()
        {
            var dbUser = await _ffaContext.GetUserAsync(Context.User as IGuildUser);

            await Context.DmAsync($"You currently have {dbUser.Reputation} reputation.");
            await Context.ReplyAsync($"You have been DMed with your reputation.");
        }
    }
}
