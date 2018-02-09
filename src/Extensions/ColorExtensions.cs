using Discord;
using FFA.Common;

namespace FFA.Extensions
{
    public static class ColorExtensions
    {
        public static string GetFormattedString(this Color color)
            => $"#{color.RawValue.ToString($"X{Configuration.MAX_HEX_LENGTH}")}";
    }
}
