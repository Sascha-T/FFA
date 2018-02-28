using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FFA.Readers
{
    public class TimeSpanReader : TypeReader
    {
        private readonly Regex numberRegex = new Regex(@"^\d+(\.\d+)?", RegexOptions.ECMAScript);
        private readonly Dictionary<string, double> timeMultipliers = new Dictionary<string, double>()
        {
            { "ms", TimeSpan.TicksPerMillisecond },
            { "s", TimeSpan.TicksPerSecond },
            { "m", TimeSpan.TicksPerMinute },
            { "d", TimeSpan.TicksPerDay },
        };

        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var numberMatch = numberRegex.Match(input);

            if (!numberMatch.Success || !ushort.TryParse(numberMatch.Value, out ushort result))
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "You have provided an invalid time."));

            var span = TimeSpan.FromHours(result);

            foreach (var pair in timeMultipliers)
            {
                if (input.EndsWith(pair.Key))
                {
                    span = TimeSpan.FromTicks(result) * pair.Value;
                    break;
                }
            }

            return Task.FromResult(TypeReaderResult.FromSuccess(span));
        }
    }
}
