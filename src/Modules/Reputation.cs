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
    [GuildOnly]
    public sealed class Reputation : ModuleBase<Context>
    {
        private readonly ReputationService _repService;
        private readonly IMongoCollection<User> _userCollection;

        public Reputation(ReputationService repService, IMongoCollection<User> userCollection)
        {
            _repService = repService;
            _userCollection = userCollection;
        }

        [Command("Rep")]
        [Summary("Give reputation to any user.")]
        public async Task RepAsync([Summary("AlabamaTrigger#0001")] [Cooldown(Configuration.REP_COOLDOWN)] [NoSelf] IGuildUser user)
        {
            await _userCollection.UpsertUserAsync(user, x => x.Reputation += Configuration.REP_INCREASE);
            await Context.ReplyAsync($"You have successfully repped {user.Bold()}.");
        }

        [Command("UnRep")]
        [Summary("Remove reputation from any user.")]
        public async Task UnRepAsync([Summary("PapaFag#6666")] [Cooldown(Configuration.UNREP_COOLDOWN)] [NoSelf] IGuildUser user)
        {
            await _userCollection.UpsertUserAsync(user, x => x.Reputation -= Configuration.UNREP_DECREASE);
            await Context.ReplyAsync($"You have successfully unrepped {user.Bold()}.");
        }

        [Command("GetRep")]
        [Alias("GetRank")]
        [Summary("Get anyone's reputation.")]
        public async Task GetRepAsync([Summary("Black Nugs#1234")] [Remainder] IGuildUser user = null)
        {
            user = user ?? Context.GuildUser;

            var dbUser = user.Id == Context.User.Id ? Context.DbUser : await _userCollection.GetUserAsync(user.Id, user.GuildId);
            var guildDbUsers = await _userCollection.WhereAsync(x => x.GuildId == Context.Guild.Id);
            var orderedDbUsers = guildDbUsers.OrderByDescending(x => x.Reputation).ToArray();
            var position = Array.FindIndex(orderedDbUsers, x => x.UserId == user.Id) + 1;

            await Context.SendAsync($"**Reputation:** {dbUser.Reputation}\n**Rank:** #{position}", $"{user}'s Reputation");
        }

        [Command("RepLeaderboards")]
        [Alias("replb", "top", "toprep")]
        [Summary("The most reputable users.")]
        public async Task RepLeaderboardsAsync()
        {
            var guildDbUsers = await _userCollection.WhereAsync(x => x.GuildId == Context.Guild.Id);
            var orderedDbUsers = guildDbUsers.OrderByDescending(x => x.Reputation).ToArray();
            var description = string.Empty;

            for (int i = 0; i < orderedDbUsers.Length;)
            {
                if (i == Configuration.LB_COUNT_DEFAULT)
                    break;

                var user = await Context.Guild.GetUserAsync(orderedDbUsers[i].UserId);

                if (user != null)
                    description += $"{(i + 1)}. **{user}:** {orderedDbUsers[i++].Reputation}\n";
            }

            await Context.SendAsync(description, "The Most Reputable Users");
        }

        [Command("UnRepLeaderboards")]
        [Alias("unreplb", "bottomrep", "bottom")]
        [Summary("The least reputable users.")]
        public async Task UnRepLeaderboardsAsync()
        {
            // TODO: helper method for leaderboard commands
            var guildDbUsers = await _userCollection.WhereAsync(x => x.GuildId == Context.Guild.Id);
            var orderedDbUsers = guildDbUsers.OrderBy(x => x.Reputation).ToArray();
            var description = string.Empty;

            for (int i = 0; i < orderedDbUsers.Length;)
            {
                if (i == Configuration.LB_COUNT_DEFAULT)
                    break;

                var user = await Context.Guild.GetUserAsync(orderedDbUsers[i].UserId);

                if (user != null)
                    description += $"{(i + 1)}. **{user}:** {orderedDbUsers[i++].Reputation}\n";
            }

            await Context.SendAsync(description, "The Least Reputable Users");
        }
    }
}
