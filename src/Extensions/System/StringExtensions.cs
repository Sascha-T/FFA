using System.Linq;

namespace FFA.Extensions.System
{
    public static class StringExtensions
    {
        public static string UpperFirstChar(this string input)
            => input.Any() ? input.First().ToString().ToUpper() + input.Substring(1) : string.Empty;
    }
}
