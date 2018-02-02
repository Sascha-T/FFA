using Discord.Commands;
using FFA.Database;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Readers
{
    public class RuleTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            input = input.ToLower();

            if (input.Length != 2 || !int.TryParse(input[0].ToString(), out int categoryNumber))
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "You have provided an invalid rule format."));
            }

            var ffaContext = services.GetRequiredService<FFAContext>();
            var groups = ffaContext.Rules.OrderBy((x) => x.Category).GroupBy((x) => x.Category).ToArray();
            
            if (groups.Length < categoryNumber || categoryNumber <= 0)
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "You have provided an invalid rule category number."));
            }

            var group = groups[categoryNumber - 1].OrderBy((x) => x.Content).ToArray();

            if (input[1] < 'a' || input[1] > 'a' + group.Length - 1)
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "You have provided an invalid rule letter."));
            }

            return Task.FromResult(TypeReaderResult.FromSuccess(group[input[1] - 'a']));
        }
    }
}
