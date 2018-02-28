using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Database.Models;
using FFA.Extensions.Database;
using FFA.Preconditions.Command;
using FFA.Services;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace FFA.Modules
{
    [Name("Owner")]
    [GuildOwner]
    public sealed class Owner : ModuleBase<Context>
    {
        private readonly RulesService _rulesService;
        private readonly IMongoCollection<Guild> _guildCollection;
        private readonly IMongoCollection<Rule> _ruleCollection;

        public Owner(RulesService rulesService, IMongoDatabase db)
        {
            _rulesService = rulesService;
            _guildCollection = db.GetCollection<Guild>("guilds");
            _ruleCollection = db.GetCollection<Rule>("rules");
        }

        [Command("SetLogChannel")]
        [Alias("setlogs", "setmodlog", "setmodlogs")]
        [Summary("Sets the log channel.")]
        public async Task SetLogChannelAsync([Summary("OldManJenkins")] [Remainder] ITextChannel logChannel)
        {
            await _guildCollection.UpsertGuildAsync(Context.Guild.Id, x => x.LogChannelId = logChannel.Id);
            await Context.ReplyAsync($"You have successfully set to log channel to {logChannel.Mention}.");
        }

        [Command("SetRulesChannel")]
        [Alias("setrules")]
        [Summary("Sets the rules channel.")]
        public async Task SetRulesChannelAsync([Summary("MrsPuff")] [Remainder] ITextChannel rulesChannel)
        {
            await _guildCollection.UpsertGuildAsync(Context.Guild.Id, x => x.RulesChannelId = rulesChannel.Id);
            await Context.ReplyAsync($"You have successfully set to rules channel to {rulesChannel.Mention}.");
        }

        [Command("SetMutedRole")]
        [Alias("setmuted", "setmuterole", "setmute")]
        [Summary("Sets the muted role.")]
        public async Task SetMutedRoleAsync([Summary("BarnacleBoy")] [Remainder] IRole mutedRole)
        {
            await _guildCollection.UpsertGuildAsync(Context.Guild.Id, x => x.MutedRoleId = mutedRole.Id);
            await Context.ReplyAsync($"You have successfully set to muted role to {mutedRole.Mention}.");
        }

        [Command("AddRule")]
        [Summary("Adds a rule.")]
        public async Task AddRuleAsync([Summary("\"Cracking your willy in broad daylight\"")] string content,
                                       [Summary("Harassment")] string category,
                                       [Summary("72h")] TimeSpan? maxMuteLength = null)
        {
            await _ruleCollection.InsertOneAsync(new Rule(Context.Guild.Id, content, category, maxMuteLength));
            await Context.ReplyAsync($"You have successfully added a new rule.");
            await _rulesService.UpdateAsync(Context.Guild);
        }

        [Command("ModifyRule")]
        [Alias("modrule", "editrule", "changerule")]
        [Summary("Modifies any rule.")]
        public async Task ModifyRuleAsync([Summary("3b")] Rule rule,
                                          [Summary("\"Nutting faster than Willy Wonka\"")] string content,
                                          [Summary("420h")] TimeSpan? maxMuteLength = null)
        {
            await _ruleCollection.UpdateAsync(rule, x =>
            {
                x.Content = content;
                x.MaxMuteLength = maxMuteLength;
            });
            await Context.ReplyAsync($"You have successfully modified this rule.");
            await _rulesService.UpdateAsync(Context.Guild);
        }

        [Command("RemoveRule")]
        [Alias("deleterule")]
        [Summary("Removes any rule.")]
        public async Task RemoveRuleAsync([Summary("2d")] Rule rule)
        {
            await _ruleCollection.DeleteOneAsync(rule);
            await Context.ReplyAsync($"You have successfully removed this rule.");
            await _rulesService.UpdateAsync(Context.Guild);
        }
    }
}
