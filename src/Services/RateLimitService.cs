using System;
using System.Collections.Concurrent;

namespace FFA.Services
{
    public class RateLimitService
    {
        private readonly ConcurrentDictionary<ulong, DateTimeOffset> _ignoredUsers = new ConcurrentDictionary<ulong, DateTimeOffset>();

        public void IgnoreUser(ulong userId, TimeSpan length)
        {
            _ignoredUsers.TryAdd(userId, DateTimeOffset.UtcNow.Add(length));
        }

        public bool IsIgnored(ulong userId)
        {
            if (!_ignoredUsers.TryGetValue(userId, out DateTimeOffset endsAt))
                return false;

            if (endsAt.Subtract(DateTimeOffset.UtcNow).Ticks <= 0)
            {
                _ignoredUsers.TryRemove(userId, out endsAt);
                return false;
            }

            return true;
        }
    }
}
