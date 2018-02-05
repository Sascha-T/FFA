using Discord;
using FFA.Common;
using FFA.Database;
using FFA.Database.Models;
using FFA.Extensions;
using System;
using System.Threading.Tasks;

namespace FFA.Services
{
    public class ModerationService
    {
        private readonly FFAContext _ffaContext;

        public ModerationService(FFAContext ffaContext)
        {
            _ffaContext = ffaContext;
        }
        
        public Task LogMuteAsync(IGuild guild, IUser moderator, IUser subject, Rule rule, TimeSpan length, string reason = null)
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

            return LogAsync(guild, author, description, Configuration.MUTE_COLOR);
        }
        
        public Task LogUnmuteAsync(IGuild guild, IUser moderator, IUser subject, string reason = null)
        {
            var author = new EmbedAuthorBuilder
            {
                Name = $"{moderator} ({moderator.Id})",
                IconUrl = moderator.GetAvatarUrl()
            };

            var description = $"**Action:** Unmute\n" +
                              $"**User:** {subject} ({subject.Id})\n" +
                              (string.IsNullOrWhiteSpace(reason) ? "" : $"**Reason:** {reason}");

            return LogAsync(guild, author, description, Configuration.UNMUTE_COLOR);
        }

        public async Task LogAsync(IGuild guild, EmbedAuthorBuilder author, string description, Color color)
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
                Author = author,
                Timestamp = DateTimeOffset.Now,
                Footer = new EmbedFooterBuilder { Text = $"Case #{dbGuild.LogCase}" },
                Description = description,
                Color = color
            };

            await logChannel.SendMessageAsync("", false, builder.Build());
            await _ffaContext.UpdateAsync(dbGuild, x => x.LogCase++);
        }
    }
}
