using System;
using System.Threading.Tasks;
using Discord.Commands;
using FFA.Entities.CustomCmd;

namespace FFA.Readers
{
    public sealed class CmdResponseReader : TypeReader
    {
        public Type Type { get; } = typeof(CmdResponse);

        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var cmdResponse = new CmdResponse(services, input);

            if (cmdResponse.Value.Length == 0)
                return Task.FromResult(TypeReaderResult.FromError(CommandError.Unsuccessful, "You have provided an invalid command response."));

            return Task.FromResult(TypeReaderResult.FromSuccess(cmdResponse));
        }
    }
}
