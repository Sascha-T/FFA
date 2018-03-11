using FFA.Common;
using System.Linq;

namespace FFA.Extensions.System
{
    public static class StringExtensions
    {
        public static string UpperFirstChar(this string input)
            => input.Any() ? input.First().ToString().ToUpper() + input.Substring(1) : string.Empty;

        public static string Bold(this string input)
            => $"**{Config.MARKDOWN_REGEX.Replace(input.ToString(), "")}**";
    }
}
