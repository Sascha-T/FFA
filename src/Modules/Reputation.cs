using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Database;
using FFA.Extensions;
using FFA.Preconditions;
using System.Threading.Tasks;

namespace FFA.Modules
{
    public sealed class Reputation : ModuleBase<Context>
    {
        private readonly FFAContext _ffaContext;

        public Reputation(FFAContext ffaContext)
        {
            _ffaContext = ffaContext;
        }

        [Command("addrule")]
        public async Task AddRule(string category, bool bannable, string message, System.TimeSpan? maxnut = null)
        {
            await _ffaContext.Rules.AddAsync(new Database.Models.Rule()
            {
                Category = category,
                Description = message,
                Bannable = bannable,
                MaxMuteLength = maxnut
            });
            await _ffaContext.SaveChangesAsync();
            await Context.ReplyAsync($"You have successfully added a new rule.");
        }

        [Command("rep")]
        [Summary("Give reputation to any user.")]
        public async Task RepAsync([Summary("NiceFaggot#0001")] [Cooldown(24)] IUser user)
        {
            await _ffaContext.UpsertUserAsync(user.Id, (x) => x.Reputation++);
            await Context.ReplyAsync($"You have successfully repped {user.Tag()}.");
        }

        [Command("unrep")]
        [Summary("Remove reputation from any user.")]
        public async Task UnrepAsync([Summary("AlabamaTrigger#6666")] [Cooldown(24)] IUser user)
        {
            await _ffaContext.UpsertUserAsync(user.Id, (x) => x.Reputation--);
            await Context.ReplyAsync($"You have successfully unrepped {user.Tag()}.");
        }

        [Command("myrep")]
        [Summary("Get your current reputation.")]
        public async Task MyrepAsync()
        {
            var dbUser = await _ffaContext.GetUserAsync(Context.User.Id);

            await Context.DmAsync($"You currently have {dbUser.Reputation} reputation.");
            await Context.ReplyAsync($"You have been DMed with your reputation.");
        }
    }
}
