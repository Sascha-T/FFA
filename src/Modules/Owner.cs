using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Database;
using FFA.Database.Models;
using FFA.Preconditions;
using FFA.Services;
using System;
using System.Threading.Tasks;

namespace FFA.Modules
{
    [Name("Owner")]
    //[GuildOwner]
    public sealed class Owner : ModuleBase<Context>
    {
        private readonly FFAContext _ffaContext;
        private readonly RulesService _rulesService;

        public Owner(FFAContext ffaContext, RulesService rulesService)
        {
            _ffaContext = ffaContext;
            _rulesService = rulesService;
        }

        [Command("SetLogChannel")]
        [Alias("setlogs", "setmodlog", "setmodlogs")]
        [Summary("Sets the log channel.")]
        public async Task SetLogChannel([Summary("OldManJenkins")] [Remainder] ITextChannel logChannel)
        {
            await _ffaContext.UpsertGuildAsync(Context.Guild.Id, x => x.LogChannelId = logChannel.Id);
            await Context.ReplyAsync($"You have successfully set to log channel to {logChannel.Mention}.");
        }

        [Command("SetRulesChannel")]
        [Alias("setrules")]
        [Summary("Sets the rules channel.")]
        public async Task SetRulesChannel([Summary("MrsPuff")] [Remainder] ITextChannel rulesChannel)
        {
            await _ffaContext.UpsertGuildAsync(Context.Guild.Id, x => x.RulesChannelId = rulesChannel.Id);
            await Context.ReplyAsync($"You have successfully set to rules channel to {rulesChannel.Mention}.");
        }

        [Command("SetMutedRole")]
        [Alias("setmuted", "setmuterole", "setmute")]
        [Summary("Sets the rules channel.")]
        public async Task SetMutedRole([Summary("BarnacleBoy")] [Remainder] IRole mutedRole)
        {
            await _ffaContext.UpsertGuildAsync(Context.Guild.Id, x => x.MutedRoleId = mutedRole.Id);
            await Context.ReplyAsync($"You have successfully set to muted role to {mutedRole.Mention}.");
        }

        [Command("AddRule")]
        [Summary("Adds a rule.")]
        public async Task AddRule([Summary("\"Sending 10 images in 5 seconds\"")] string content,
                                  [Summary("Spam")] string category,
                                  [Summary("24h")] TimeSpan? maxMuteLength = null)
        {
            await _ffaContext.AddAsync(new Rule(Context.Guild.Id, content, category, maxMuteLength));
            await Context.ReplyAsync($"You have successfully added a new rule.");
            await _rulesService.UpdateAsync(Context.Guild);
        }
    }
}
