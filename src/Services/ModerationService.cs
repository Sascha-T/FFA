using Discord;
using FFA.Common;
using FFA.Database;
using FFA.Database.Models;
using FFA.Extensions;
using System;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class ModerationService
    {
        private readonly FFAContext _ffaContext;

        public ModerationService(FFAContext ffaContext)
        {
            _ffaContext = ffaContext;
        }

        public Task LogMuteAsync(IGuild guild, IUser moderator, IUser subject, Rule rule, TimeSpan length, string reason = null)
        {
            var description = $"**Action:** Mute\n" +
                              $"**User:** {subject} ({subject.Id})\n" +
                              $"**Rule:** {rule.Content}\n" +
                              (string.IsNullOrWhiteSpace(reason) ? "" : $"**Reason:** {reason}\n") +
                              $"**Length:** {length.TotalHours}h";

            return LogAsync(guild, description, Configuration.MUTE_COLOR, moderator);
        }

        public Task LogUnmuteAsync(IGuild guild, IUser moderator, IUser subject, string reason = null)
        {
            var description = $"**Action:** Unmute\n" +
                              $"**User:** {subject} ({subject.Id})\n" +
                              (string.IsNullOrWhiteSpace(reason) ? "" : $"**Reason:** {reason}");

            return LogAsync(guild, description, Configuration.UNMUTE_COLOR, moderator);
        }

        public Task LogAutoUnmuteAsync(IGuild guild, IUser subject)
            => LogAsync(guild, $"**Action:** Automatic Unmute\n**User:** {subject} ({subject.Id})\n", Configuration.UNMUTE_COLOR);

        public Task LogClearAsync(IGuild guild, IUser moderator, IUser subject, Rule rule, int quantity, string reason = null)
        {
            var description = $"**Action:** Clear\n" +
                              $"**User:** {subject} ({subject.Id})\n" +
                              $"**Rule:** {rule.Content}\n" +
                              $"**Quantity:** {quantity}\n" +
                              (string.IsNullOrWhiteSpace(reason) ? "" : $"**Reason:** {reason}\n");

            return LogAsync(guild, description, Configuration.CLEAR_COLOR, moderator);
        }

        public async Task LogAsync(IGuild guild, string description, Color color, IUser moderator = null)
        {
            var dbGuild = await _ffaContext.GetGuildAsync(guild.Id);

            if (!dbGuild.LogChannelId.HasValue)
            {
                return;
            }

            var logChannel = await guild.GetChannelAsync(dbGuild.LogChannelId.Value) as ITextChannel;

            if (logChannel == null || !await logChannel.CanSendAsync())
            {
                return;
            }

            var builder = new EmbedBuilder()
            {
                Timestamp = DateTimeOffset.Now,
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
            await _ffaContext.UpdateAsync(dbGuild, x => x.LogCase++);
        }
    }
}
