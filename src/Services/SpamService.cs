using Discord;
using FFA.Common;
using FFA.Database.Models;
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
    public sealed class SpamService
    {
        private readonly ModerationService _moderationService;
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<Mute> _muteCollection;
        private readonly ConcurrentDictionary<ulong, SpamEntry> _spamEntries = new ConcurrentDictionary<ulong, SpamEntry>();

        public SpamService(ModerationService moderationService, IMongoDatabase db)
        {
            _moderationService = moderationService;
            _userCollection = db.GetCollection<User>("users");
            _muteCollection = db.GetCollection<Mute>("mutes");
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
            await _muteCollection.InsertOneAsync(new Mute(context.Guild.Id, context.User.Id, Config.SPAM_MUTE_LENGTH));
            await _moderationService.LogAutoMuteAsync(context, Config.SPAM_MUTE_LENGTH);
            await _userCollection.UpdateAsync(context.DbUser, x => x.Reputation -= Config.SPAM_REP_PENALTY);

            return false;
        }
    }

    public sealed class SpamEntry
    {
        public SpamEntry(IUserMessage message)
        {
            FirstTimestamp = message.Timestamp;
        }

        public int Count { get; set; } = 1;
        public DateTimeOffset FirstTimestamp { get; }
    }
}
