using Discord;
using FFA.Common;
using FFA.Database;
using FFA.Database.Models;
using System;
using System.Threading.Tasks;

namespace FFA.Services
{
    public class ModerationService
    {
        private readonly FFAContext _ffaContext;
        private readonly Configuration _config;

        public ModerationService(FFAContext ffaContext, Configuration config)
        {
            _ffaContext = ffaContext;
            _config = config;
        }

        // TODO: color from config
        public Task LogMute(IGuild guild, IUser moderator, IUser subject, Rule rule, TimeSpan length, string reason = null)
        {
            var author = new EmbedAuthorBuilder
            {
                Name = $"{moderator} ({moderator.Id})",
                IconUrl = moderator.GetAvatarUrl()
            };

            var description = $"**Action:** Mute\n" +
                              $"**User:** {subject} ({subject.Id})\n" +
                              $"**Rule:** {rule.Content}\n" +
                              (string.IsNullOrWhiteSpace(reason) ? "" : $"**Reason:** {reason}\n") +
                              $"**Length:** {length.TotalHours}h";

            return Log(guild, author, description, _config.MuteColor);
        }

        // TODO: color from config
        public Task LogUnmute(IGuild guild, IUser moderator, IUser subject, string reason = null)
        {
            var author = new EmbedAuthorBuilder
            {
                Name = $"{moderator} ({moderator.Id})",
                IconUrl = moderator.GetAvatarUrl()
            };

            var description = $"**Action:** Unmute\n" +
                              $"**User:** {subject} ({subject.Id})\n" +
                              (string.IsNullOrWhiteSpace(reason) ? "" : $"**Reason:** {reason}");

            return Log(guild, author, description, _config.UnmuteColor);
        }

        public async Task Log(IGuild guild, EmbedAuthorBuilder author, string description, Color color)
        {
            var dbGuild = await _ffaContext.GetGuildAsync(guild.Id);

            if (dbGuild.LogChannelId.HasValue)
            {
                var logChannel = await guild.GetChannelAsync(dbGuild.LogChannelId.Value) as ITextChannel;

                var builder = new EmbedBuilder()
                {
                    Author = author,
                    Timestamp = DateTimeOffset.Now,
                    Footer = new EmbedFooterBuilder{ Text = $"Case #{dbGuild.LogCase}" },
                    Description = description,
                    Color = color
                };

                await logChannel.SendMessageAsync("", false, builder.Build());
                await _ffaContext.UpdateAsync(dbGuild, x => x.LogCase++);
            }
        }
    }
}
