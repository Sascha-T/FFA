using System.Text.RegularExpressions;

namespace FFA.Extensions
{
    public static class StringExtensions
    {
        static string PascalCaseToSentence(string input)
        {
            return Regex.Replace(Regex.Replace(input, @"\w+", x => $"{char.ToLower(x.Value[0])}{x.Value.Substring(1)}"), @"\s+", string.Empty);
        }
    }
}
