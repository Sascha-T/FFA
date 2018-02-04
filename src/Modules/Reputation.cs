﻿using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Database;
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
        private readonly FFAContext _ffaContext;
        private readonly ReputationService _repService;

        public Reputation(FFAContext ffaContext, ReputationService repService)
        {
            _ffaContext = ffaContext;
            _repService = repService;
        }

        [Command("Mod")]
        [Summary("Informs you whether or not you are a moderator.")]
        public Task Mod()
        {
            if (_repService.IsInTop(20, Context.User.Id, Context.Guild.Id))
            {
                return Context.ReplyAsync("You are currently a moderator.");
            }
            else
            {
                return Context.ReplyErrorAsync("You are currently not a moderator.");
            }
        }

        [Command("Rep")]
        [Summary("Give reputation to any user.")]
        public async Task RepAsync([Summary("AlabamaTrigger#0001")] [Cooldown(24)] [NoSelf] IGuildUser user)
        {
            await _ffaContext.UpsertUserAsync(user, x => x.Reputation++);
            await Context.ReplyAsync($"You have successfully repped {user.Bold()}.");
        }

        [Command("UnRep")]
        [Summary("Remove reputation from any user.")]
        public async Task UnRepAsync([Summary("PapaFag#6666")] [Cooldown(24)] [NoSelf] IGuildUser user)
        {
            await _ffaContext.UpsertUserAsync(user, x => x.Reputation--);
            await Context.ReplyAsync($"You have successfully unrepped {user.Bold()}.");
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