using Discord;
using FFA.Entities.Service;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FFA.Services
{
    public sealed class DeletedMessagesService : Service
    {
        private readonly ConcurrentDictionary<ulong, ConcurrentBag<IUserMessage>> _deletedMsgs =
            new ConcurrentDictionary<ulong, ConcurrentBag<IUserMessage>>();

        public void Add(ulong channelId, IUserMessage msg)
        {
            var channelMsgs = _deletedMsgs.GetOrAdd(channelId, x => new ConcurrentBag<IUserMessage>());

            channelMsgs.Add(msg);
        }

        public IReadOnlyList<IUserMessage> GetLast(ulong channelId, int quantity)
        {
            if (!_deletedMsgs.TryGetValue(channelId, out ConcurrentBag<IUserMessage> channelMsgs))
                return Enumerable.Empty<IUserMessage>().ToImmutableArray();

            return channelMsgs.OrderByDescending(x => x.Timestamp).Take(quantity).ToImmutableArray();
        }
    }
}
