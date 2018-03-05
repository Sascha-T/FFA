using Discord;
using FFA.Common;
using FFA.Database.Models;
using FFA.Entities.Service;
using FFA.Extensions.Database;
using FFA.Extensions.Discord;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class ModerationService : Service
    {
        private readonly IMongoCollection<Guild> _dbGuilds;
        private readonly IMongoCollection<Mute> _dbMutes;
        private readonly SendingService _sender;

        public ModerationService(IMongoCollection<Guild> dbGuilds, IMongoCollection<Mute> dbMutes, SendingService sender)
        {
            _dbGuilds = dbGuilds;
            _dbMutes = dbMutes;
            _sender = sender;
        }

        public Task InformUserAsync(Context ctx, IUser subject, Rule rule, TimeSpan length, string reason = null)
        {
            reason = string.IsNullOrWhiteSpace(reason) ? "" : $"\n{ctx.User.Bold()} has provided the following reason:```\n{reason}```";

            // TODO: prettier solution for this long message (SAME FOR HELP ASWELL)
            // TODO: sterilize reason to remove discord markdown chars
            return _sender.TryDMAsync(subject,
                $"{ctx.User.Bold()} has muted you for **{length.TotalHours}h** for breaking the following rule:" +
                $"```\n{rule.Content}```{reason}\n**If this mute was unjustified or invalid, there are several steps you must take to " +
                $"vindicate yourself:**\n\n**1.** You must `{Config.PREFIX}unrep \"{ctx.User}\"`. This is essential as it will prevent " +
                $"{ctx.User.Bold()} from unjustly muting others. This command must be used inside a guild channel. If there are no " +
                $"channels dedicated to allow muted users to use commands, please contact the guild owner.\n\n**2.** You  must " +
                $"use `{Config.PREFIX}replb 30` to DM these moderators with undeniable proof of your innocence. You must explain " +
                $"in detail why {ctx.User.Bold()}'s mute was invalid, and why it should be reverted.\n\n**3.** You must ensure " +
                $"{ctx.User.Bold()} get's muted for unlawful punishment. You may do this by lobbying other moderators, or by gaining " +
                $"enough reputation to do it yourself. If there is no unlawful punishment rule, create a poll which adds it, and lobby " +
                $"other users to vote in favor of said poll.", guild: ctx.Guild);
        }

        public async Task CreateMute(Context ctx, IUser user, Rule rule, TimeSpan length, string reason = null)
        {
            await _dbMutes.InsertOneAsync(new Mute(ctx.Guild.Id, user.Id, length));
            await LogMuteAsync(ctx, user, rule, length, reason);
            await InformUserAsync(ctx, user, rule, length, reason);
        }

        public async Task RemoveMute(Context ctx, IUser user, string reason)
        {
            await _dbMutes.UpdateManyAsync(x => x.UserId == user.Id && x.GuildId == ctx.Guild.Id,
                        new UpdateDefinitionBuilder<Mute>().Set(x => x.Active, false));
            await LogUnmuteAsync(ctx, user, reason);
        }

        public Task LogMuteAsync(Context ctx, IUser subject, Rule rule, TimeSpan length, string reason = null)
        {
            var elements = new List<(string, string)>
            {
                ("Action", "Mute"),
                ("User", $"{subject} ({subject.Id})"),
                ("Rule", rule.Content),
                ("Length", $"{length.TotalHours}h")
            };

            if (!string.IsNullOrWhiteSpace(reason))
                elements.Add(("Reason", reason));

            return LogAsync(ctx.Guild, elements, Config.MUTE_COLOR, ctx.User);
        }

        public Task LogUnmuteAsync(Context ctx, IUser subject, string reason)
        {
            var elements = new(string, string)[]
            {
                ("Action", "Unmute"),
                ("User", $"{subject} ({subject.Id})"),
                ("Reason", reason)
            };

            return LogAsync(ctx.Guild, elements, Config.UNMUTE_COLOR, ctx.User);
        }

        public Task LogAutoMuteAsync(Context ctx, TimeSpan length)
            => LogAsync(ctx.Guild, new(string, string)[]
            {
                ("Action", "Automatic Mute"),
                ("User", $"{ctx.User} ({ctx.User.Id})"),
                ("Length", $"{length.TotalHours}h")
            }, Config.MUTE_COLOR);

        public Task LogAutoUnmuteAsync(IGuild guild, IUser subject)
            => LogAsync(guild, new(string, string)[]
            {
                ("Action", "Automatic Unmute"),
                ("User", $"{subject} ({subject.Id})")
            }, Config.UNMUTE_COLOR);

        public Task LogClearAsync(Context ctx, IUser subject, Rule rule, int quantity, string reason = null)
        {
            var elements = new List<(string, string)>
            {
                ("Action", "Clear"),
                ("User", $"{subject} ({subject.Id})"),
                ("Rule", rule.Content),
                ("Quantity", quantity.ToString())
            };

            if (!string.IsNullOrWhiteSpace(reason))
                elements.Add(("Reason", reason));

            return LogAsync(ctx.Guild, elements, Config.CLEAR_COLOR, ctx.User);
        }

        public async Task LogAsync(IGuild guild, IReadOnlyCollection<(string, string)> elements, Color color, IUser moderator = null)
        {
            var dbGuild = await _dbGuilds.GetGuildAsync(guild.Id);

            await _dbGuilds.UpdateAsync(dbGuild, x => x.LogCase++);

            if (!dbGuild.LogChannelId.HasValue)
                return;

            var logChannel = await guild.GetTextChannelAsync(dbGuild.LogChannelId.Value);

            if (logChannel == null || !await logChannel.CanSendAsync())
                return;

            var description = string.Empty;

            foreach (var element in elements)
                description += $"**{element.Item1}:** {element.Item2}\n";

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

            await logChannel.SendMessageAsync(string.Empty, false, builder.Build());
        }
    }
}
