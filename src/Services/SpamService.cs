using Discord;
using FFA.Common;
using FFA.Database.Models;
using FFA.Extensions;
using FFA.Utility;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class SpamService
    {
        private readonly ModerationService _moderationService;
        private readonly ConcurrentDictionary<ulong, SpamEntry> _spamEntries = new ConcurrentDictionary<ulong, SpamEntry>();

        public SpamService(ModerationService moderationService)
        {
            _moderationService = moderationService;
        }
        
        public async Task Authenticate(Context context)
        {
            if (_spamEntries.TryGetValue(context.User.Id, out SpamEntry entry))
            {
                if (DateTimeOffset.Now.Subtract(entry.FirstTimestamp) <= Configuration.SPAM_DURATION)
                {
                    if (Similarity.Compare(context.Message.Content, entry.LastContent, Configuration.SPAM_SIMILARITY))
                    {
                        entry.Count++;
                        entry.LastContent = context.Message.Content;

                        if (entry.Count >= Configuration.SPAM_LIMIT)
                        {
                            await context.Db.UpdateAsync(context.DbUser, x => x.Reputation -= Configuration.SPAM_REP_PENALTY);

                            if (context.DbGuild.MutedRoleId.HasValue)
                            {
                                var mutedRole = context.Guild.GetRole(context.DbGuild.MutedRoleId.Value);

                                if (mutedRole != null && await mutedRole.CanUseAsync() &&
                                    !context.GuildUser.RoleIds.Any(x => x == mutedRole.Id))
                                {
                                    await context.GuildUser.AddRoleAsync(mutedRole);
                                    await context.Db.AddAsync(new Mute(context.Guild.Id, context.User.Id,
                                                              DateTimeOffset.Now.Add(Configuration.SPAM_MUTE_LENGTH)));
                                    await _moderationService.LogAutoMuteAsync(context, Configuration.SPAM_MUTE_LENGTH);
                                }
                            }
                        }
                    }
                }
                else
                {
                    _spamEntries.TryUpdate(context.User.Id, new SpamEntry(context.Message), entry);
                }
            }
            else
            {
                _spamEntries.TryAdd(context.User.Id, new SpamEntry(context.Message));
            }
        }
    }

    public sealed class SpamEntry
    {
        public SpamEntry(IUserMessage message)
        {
            LastContent = message.Content;
            FirstTimestamp = message.Timestamp;
        }

        public int Count { get; set; } = 1;
        public DateTimeOffset FirstTimestamp { get; }
        public string LastContent { get; set; }
    }
}
