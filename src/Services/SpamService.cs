using FFA.Common;
using FFA.Database.Models;
using FFA.Entities.Service;
using FFA.Entities.Spam;
using FFA.Extensions.Database;
using FFA.Extensions.Discord;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

// TODO: allow certain channels to be ignored
namespace FFA.Services
{
    // TODO: ensure all async methods are suffixed with Async
    public sealed class SpamService : Service
    {
        private readonly ModerationService _modService;
        private readonly IMongoCollection<User> _dbUsers;
        private readonly IMongoCollection<Mute> _dbMutes;
        private readonly ConcurrentDictionary<ulong, SpamEntry> _spamEntries = new ConcurrentDictionary<ulong, SpamEntry>();

        public SpamService(ModerationService modService, IMongoCollection<User> dbUsers, IMongoCollection<Mute> dbMutes)
        {
            _modService = modService;
            _dbUsers = dbUsers;
            _dbMutes = dbMutes;
        }

        public async Task<bool> AuthenticateAsync(Context context)
        {
            if (!_spamEntries.TryGetValue(context.User.Id, out SpamEntry entry))
            {
                _spamEntries.TryAdd(context.User.Id, new SpamEntry(context.Message));
                return true;
            }
            else if (DateTimeOffset.UtcNow.Subtract(entry.FirstTimestamp) > Config.SPAM_DURATION)
            {
                _spamEntries.TryUpdate(context.User.Id, new SpamEntry(context.Message), entry);
                return true;
            }

            entry.Count++;

            if (entry.Count < Config.SPAM_LIMIT)
                return true;
            else if (!context.DbGuild.MutedRoleId.HasValue)
                return false;

            var mutedRole = context.Guild.GetRole(context.DbGuild.MutedRoleId.Value);

            if (mutedRole == null || !await mutedRole.CanUseAsync() || context.GuildUser.RoleIds.Any(x => x == mutedRole.Id))
                return false;

            await context.GuildUser.AddRoleAsync(mutedRole);
            await _dbMutes.InsertOneAsync(new Mute(context.Guild.Id, context.User.Id, Config.SPAM_MUTE_LENGTH));
            await _modService.LogAutoMuteAsync(context, Config.SPAM_MUTE_LENGTH);
            await _dbUsers.UpdateAsync(context.DbUser, x => x.Reputation -= Config.SPAM_REP_PENALTY);

            return false;
        }
    }
}
