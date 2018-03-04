using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using FFA.Common;

namespace FFA.Readers
{
    public sealed class GuildEmoteReader : TypeReader
    {
        public Type Type { get; } = typeof(GuildEmote);

        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            if (!Config.EMOTE_REGEX.IsMatch(input))
                return Task.FromResult(TypeReaderResult.FromError(CommandError.Unsuccessful, "You have provided an invalid emote."));

            var emoteId = ulong.Parse(Config.EMOTE_ID_REGEX.Replace(input, string.Empty));
            var emote = context.Guild.Emotes.FirstOrDefault(x => x.Id == emoteId);

            if (emote == default(GuildEmote))
                return Task.FromResult(TypeReaderResult.FromError(CommandError.Unsuccessful, "This emote is not an emote of this server."));

            return Task.FromResult(TypeReaderResult.FromSuccess(emote));
        }
    }
}
