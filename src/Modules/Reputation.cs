using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Database;
using FFA.Database.Models;
using FFA.Extensions;
using FFA.Preconditions;
using System;
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

        [Command("addrule")]
        [Summary("Adds a rule.")]
        public async Task AddRule(string content, string category, TimeSpan? maxMuteLength = null)
        {
            await _ffaContext.AddAsync(new Rule(content, category, maxMuteLength));
            await Context.ReplyAsync($"You have successfully added a new rule.");
        }

        [Command("rep")]
        [Summary("Give reputation to any user.")]
        public async Task RepAsync([Summary("AlabamaTrigger#0001")] [Cooldown(24)] IUser user)
        {
            await _ffaContext.UpsertUserAsync(user.Id, (x) => x.Reputation++);
            await Context.ReplyAsync($"You have successfully repped {user.Tag()}.");
        }

        [Command("unrep")]
        [Summary("Remove reputation from any user.")]
        public async Task UnRepAsync([Summary("PapaFag#6666")] [Cooldown(24)] IUser user)
        {
            await _ffaContext.UpsertUserAsync(user.Id, (x) => x.Reputation--);
            await Context.ReplyAsync($"You have successfully unrepped {user.Tag()}.");
        }

        [Command("myrep")]
        [Summary("Get your current reputation.")]
        public async Task MyRepAsync()
        {
            var dbUser = await _ffaContext.GetUserAsync(Context.User.Id);

            await Context.DmAsync($"You currently have {dbUser.Reputation} reputation.");
            await Context.ReplyAsync($"You have been DMed with your reputation.");
        }
    }
}
