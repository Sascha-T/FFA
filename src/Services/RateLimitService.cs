using FFA.Entities.Service;
using System;
using System.Collections.Concurrent;

namespace FFA.Services
{
    public class RateLimitService : Service
    {
        private readonly ConcurrentDictionary<ulong, DateTimeOffset> _ignoredUsers = new ConcurrentDictionary<ulong, DateTimeOffset>();

        public void IgnoreUser(ulong userId, TimeSpan length)
        {
            var endsAt = DateTimeOffset.UtcNow.Add(length);
            _ignoredUsers.AddOrUpdate(userId, endsAt, (x, y) => endsAt);
        }

        public bool IsIgnored(ulong userId)
        {
            if (!_ignoredUsers.TryGetValue(userId, out DateTimeOffset endsAt))
                return false;

            if (endsAt.CompareTo(DateTimeOffset.UtcNow) < 0)
            {
                _ignoredUsers.TryRemove(userId, out endsAt);
                return false;
            }

            return true;
        }
    }
}
