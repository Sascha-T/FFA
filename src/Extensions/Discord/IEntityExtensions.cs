using Discord;

namespace FFA.Extensions.Discord
{
    public static class IEntityExtensions
    {
        public static string Bold(this IEntity<ulong> entity)
            => $"**{entity}**";
    }
}
