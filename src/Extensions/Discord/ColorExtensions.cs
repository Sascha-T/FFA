using Discord;
using FFA.Common;

namespace FFA.Extensions.Discord
{
    internal static class ColorExtensions
    {
        internal static string GetFormattedString(this Color color)
            => $"#{color.RawValue.ToString($"X{Configuration.MAX_HEX_LENGTH}")}";
    }
}
