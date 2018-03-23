using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Database.Models;
using FFA.Extensions.Database;
using FFA.Extensions.Discord;
using FFA.Preconditions.Command;
using FFA.Preconditions.Parameter;
using FFA.Services;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Modules
{
    [Name("Reputation")]
    [Summary("Commands dedicated to managing the reputation system.")]
    [GuildOnly]
    public sealed class Reputation : ModuleBase<Context>
    {
        // TODO: shorter names for like everything, especially services
        private readonly ReputationService _repService;
        private readonly LeaderboardService _lbService;
        private readonly IMongoCollection<User> _dbUsers;

        public Reputation(ReputationService repService, LeaderboardService lbService, IMongoCollection<User> dbUsers)
        {
            _repService = repService;
            _lbService = lbService;
            _dbUsers = dbUsers;
        }

        [Command("Rep")]
        [Summary("Give reputation to any user.")]
        [Cooldown(Config.REP_CD)]
        [MemberAge(Config.MEMBER_AGE)]
        public async Task RepAsync(
            [Summary("AlabamaTrigger#0001")] [NoSelf] IUser user)
        {
            await _dbUsers.UpsertUserAsync(user.Id, Context.Guild.Id, x => x.Reputation += Config.REP_INCREASE);
            await Context.ReplyAsync($"You have successfully repped {user.Bold()}.");
        }

        [Command("UnRep")]
        [Summary("Remove reputation from any user.")]
        [Cooldown(Config.UNREP_CD)]
        [MemberAge(Config.MEMBER_AGE)]
        public async Task UnRepAsync(
            [Summary("PapaJohn#6666")] [NoSelf] IUser user)
        {
            await _dbUsers.UpsertUserAsync(user.Id, Context.Guild.Id, x => x.Reputation -= Config.UNREP_DECREASE);
            await Context.ReplyAsync($"You have successfully unrepped {user.Bold()}.");
        }

        [Command("GetRep")]
        [Alias("GetRank")]
        [Summary("Get anyone's reputation.")]
        public async Task GetRepAsync(
            [Summary("Nolan#6900")] [Remainder] IUser user = null)
        {
            user = user ?? Context.GuildUser;

            var dbUser = user.Id == Context.User.Id ? Context.DbUser : await _dbUsers.GetUserAsync(user.Id, Context.Guild.Id);
            var guildDbUsers = await _dbUsers.WhereAsync(x => x.GuildId == Context.Guild.Id);
            var orderedDbUsers = guildDbUsers.OrderByDescending(x => x.Reputation).ToArray();
            var position = Array.FindIndex(orderedDbUsers, x => x.UserId == user.Id) + 1;

            await Context.SendAsync($"**Reputation:** {dbUser.Reputation.ToString("F2")}\n**Rank:** #{position}", $"{user}'s Reputation");
        }

        [Command("RepLeaderboards")]
        [Alias("replb", "top", "toprep")]
        [Summary("The most reputable users.")]
        public async Task RepLeaderboardsAsync(
            [Summary("15")] [Between(Config.MIN_LB, Config.MAX_LB)] int count = Config.LB_COUNT)
            => await Context.SendAsync(await _lbService.GetUserLbAsync(Context.Guild, x => x.Reputation, count), "The Most Reputable Users");

        [Command("UnRepLeaderboards")]
        [Alias("unreplb", "bottomrep", "bottom")]
        [Summary("The least reputable users.")]
        public async Task UnRepLeaderboardsAsync(
            [Summary("20")] [Between(Config.MIN_LB, Config.MAX_LB)] int count = Config.LB_COUNT)
            => await Context.SendAsync(await _lbService.GetUserLbAsync(Context.Guild, x => x.Reputation, count, true), "The Least Reputable Users");
    }
}
