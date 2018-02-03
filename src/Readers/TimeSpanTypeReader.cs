using Discord.Commands;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FFA.Readers
{
    public class TimeSpanTypeReader : TypeReader
    {
        private readonly Regex numberRegex = new Regex(@"^\d+(\.\d+)?");

        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var numberMatch = numberRegex.Match(input);

            if (!numberMatch.Success || !double.TryParse(numberMatch.Value, out double result) || result <= 0)
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "You have provided an invalid time."));
            }

            TimeSpan span;

            if (input.EndsWith("ms"))
            {
                span = TimeSpan.FromMilliseconds(result);
            }
            else if (input.EndsWith('s'))
            {
                span = TimeSpan.FromSeconds(result);
            }
            else if (input.EndsWith('m'))
            {
                span = TimeSpan.FromMinutes(result);
            }
            else if (input.EndsWith('d'))
            {
                span = TimeSpan.FromDays(result);
            }
            else
            {
                span = TimeSpan.FromHours(result);
            }

            return Task.FromResult(TypeReaderResult.FromSuccess(span));
        }
    }
}
