using Discord;
using Discord.Commands;
using Discord.Rest;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Readers
{
    internal class UserTypeReader : TypeReader
    {
        public Type Type { get; } = typeof(IUser);

        public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input,
            IServiceProvider services)
        {
            var restClient = services.GetRequiredService<DiscordRestClient>();

            if (MentionUtils.TryParseUser(input, out ulong id) ||
                ulong.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out id))
            {
                var user = await context.Client.GetUserAsync(id);

                if (user == null)
                    user = await restClient.GetUserAsync(id);

                if (user != null)
                    return TypeReaderResult.FromSuccess(user);
            }

            if (context.Guild != null)
            {
                var guildUsers = await context.Guild.GetUsersAsync(CacheMode.CacheOnly);

                int index = input.LastIndexOf('#');

                if (index >= 0)
                {
                    var username = input.Substring(0, index);

                    if (ushort.TryParse(input.Substring(index + 1), out ushort discriminator))
                    {
                        var guildUser = guildUsers.FirstOrDefault(x => x.DiscriminatorValue == discriminator &&
                            string.Equals(username, x.Username, StringComparison.OrdinalIgnoreCase));

                        if (guildUser != default(IGuildUser))
                            return TypeReaderResult.FromSuccess(guildUser);
                    }
                }
                else
                {
                    var results = new Dictionary<ulong, TypeReaderValue>();
                    var matches = guildUsers.Where(x =>
                        string.Equals(input, x.Username, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(input, x.Nickname, StringComparison.OrdinalIgnoreCase));

                    foreach (var match in matches)
                        results.TryAdd(match.Id, new TypeReaderValue(match, 0.7f));

                    if (results.Count > 0)
                        return TypeReaderResult.FromSuccess(results.Values);
                }
            }

            return TypeReaderResult.FromError(CommandError.ObjectNotFound, "User not found.");
        }
    }
}
