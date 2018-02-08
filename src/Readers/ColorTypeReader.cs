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
            var color = System.Drawing.Color.FromName(input);

            if (color.ToArgb() == 0)
            {
                return Task.FromResult(TypeReaderResult.FromSuccess(new Discord.Color(color.R, color.G, color.B)));
            }
            else if (uint.TryParse(input, NumberStyles.HexNumber, _numberFormat, out uint result))
            {
                return Task.FromResult(TypeReaderResult.FromSuccess(new Discord.Color(result)));
            }

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "You have provided an invalid color."));
        }
    }
}
