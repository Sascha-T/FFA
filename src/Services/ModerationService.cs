using Discord;
using FFA.Common;
using FFA.Database;
using FFA.Database.Models;
using FFA.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class ModerationService
    {
        public Task LogMuteAsync(Context context, IUser subject, Rule rule, TimeSpan length, string reason = null)
        {
            var elements = new List<(string, string)>
            {
                ("Action", "Mute"),
                ("User", $"{subject} ({subject.Id})"),
                ("Rule", rule.Content),
                ("Length", $"{length.TotalHours}h")
            };

            if (!string.IsNullOrWhiteSpace(reason))
            {
                elements.Add(("Reason", reason));
            }

            return LogAsync(context.Db, context.Guild, elements, Configuration.MUTE_COLOR, context.User);
        }

        public Task LogUnmuteAsync(Context context, IUser subject, string reason = null)
        {
            var elements = new List<(string, string)>
            {
                ("Action", "Unmute"),
                ("User", $"{subject} ({subject.Id})")
            };

            if (!string.IsNullOrWhiteSpace(reason))
            {
                elements.Add(("Reason", reason));
            }

            return LogAsync(context.Db, context.Guild, elements, Configuration.MUTE_COLOR, context.User);
        }

        public Task LogAutoMuteAsync(Context context, TimeSpan length)
            => LogAsync(context.Db, context.Guild, new(string, string)[]
            {
                ("Action", "Automatic Mute"),
                ("User", $"{context.User} ({context.User.Id})"),
                ("Length", $"{length.TotalHours}h")
            }, Configuration.MUTE_COLOR);

        public Task LogAutoUnmuteAsync(FFAContext ffaContext, IGuild guild, IUser subject)
            => LogAsync(ffaContext, guild, new (string, string)[]
            {
                ("Action", "Automatic Unmute"),
                ("User", $"{subject} ({subject.Id})")
            }, Configuration.UNMUTE_COLOR);

        public Task LogClearAsync(Context context, IUser subject, Rule rule, int quantity, string reason = null)
        {
            var elements = new List<(string, string)>
            {
                ("Action", "Clear"),
                ("User", $"{subject} ({subject.Id})"),
                ("Rule", rule.Content),
                ("Quantity", quantity.ToString())
            };

            if (!string.IsNullOrWhiteSpace(reason))
            {
                elements.Add(("Reason", reason));
            }

            return LogAsync(context.Db, context.Guild, elements, Configuration.CLEAR_COLOR, context.User);
        }

        public async Task LogAsync(FFAContext ffaContext, IGuild guild, IReadOnlyList<(string, string)> elements, Color color, IUser moderator = null)
        {
            var dbGuild = await ffaContext.GetGuildAsync(guild.Id);

            if (!dbGuild.LogChannelId.HasValue)
            {
                return;
            }

            var logChannel = await guild.GetChannelAsync(dbGuild.LogChannelId.Value) as ITextChannel;

            if (logChannel == null || !await logChannel.CanSendAsync())
            {
                return;
            }

            var description = string.Empty;

            foreach (var element in elements)
            {
                description += $"**{element.Item1}:** {element.Item2}\n";
            }

            var builder = new EmbedBuilder()
            {
                Timestamp = DateTimeOffset.UtcNow,
                Footer = new EmbedFooterBuilder { Text = $"Case #{dbGuild.LogCase}" },
                Description = description,
                Color = color
            };

            if (moderator != null)
            {
                builder.WithAuthor(new EmbedAuthorBuilder
                {
                    Name = $"{moderator} ({moderator.Id})",
                    IconUrl = moderator.GetAvatarUrl()
                });
            }

            await logChannel.SendMessageAsync("", false, builder.Build());
            await ffaContext.UpdateAsync(dbGuild, x => x.LogCase++);
        }
    }
}
