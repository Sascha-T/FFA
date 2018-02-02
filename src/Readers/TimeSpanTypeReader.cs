using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace FFA.Readers
{
    public class TimeSpanTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            if (!uint.TryParse(input, out uint result))
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "You have provided an invalid time."));
            }

            TimeSpan span;

            switch (input.ToLower()[input.Length - 1])
            {
                case 's':
                    span = TimeSpan.FromSeconds(result);
                    break;
                case 'm':
                    span = TimeSpan.FromMinutes(result);
                    break;
                case 'd':
                    span = TimeSpan.FromDays(result);
                    break;
                default:
                    span = TimeSpan.FromHours(result);
                    break;
            }

            return Task.FromResult(TypeReaderResult.FromSuccess(span));
        }
    }
}
