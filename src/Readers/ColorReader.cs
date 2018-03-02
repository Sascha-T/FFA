using Discord;
using Discord.Commands;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace FFA.Readers
{
    public class ColorReader : TypeReader
    {
        private readonly NumberFormatInfo _numberFormat = new NumberFormatInfo();

        public Type Type { get; } = typeof(Color);

        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            if (uint.TryParse(input.Replace("#", string.Empty), NumberStyles.HexNumber, _numberFormat, out uint result))
                return Task.FromResult(TypeReaderResult.FromSuccess(new Color(result)));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.Unsuccessful, "You have provided an invalid color hex value."));
        }
    }
}
