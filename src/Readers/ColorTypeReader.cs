using Discord.Commands;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace FFA.Readers
{
    public class ColorTypeReader : TypeReader
    {
        private readonly NumberFormatInfo _numberFormat = new NumberFormatInfo();

        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            // change string replace to regex replace
            if (uint.TryParse(input.Replace("#", string.Empty), NumberStyles.HexNumber, _numberFormat, out uint result))
            {
                return Task.FromResult(TypeReaderResult.FromSuccess(new Discord.Color(result)));
            }

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "You have provided an invalid color hex value."));
        }
    }
}
