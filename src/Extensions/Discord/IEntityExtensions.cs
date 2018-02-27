using Discord;

namespace FFA.Extensions.Discord
{
    internal static class IEntityExtensions
    {
        internal static string Bold(this IEntity<ulong> entity)
            => $"**{entity}**";
    }
}
