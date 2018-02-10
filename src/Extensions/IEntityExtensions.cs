using Discord;

namespace FFA.Extensions
{
    public static class IEntityExtensions
    {
        public static string Bold(this IEntity<ulong> entity)
            => $"**{entity}**";
    }
}
