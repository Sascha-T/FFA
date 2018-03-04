using Discord.Commands;
using System;

namespace FFA.Entities.Cooldown
{
    public class Cooldown
    {
        public Cooldown(ulong userId, ulong guildId, CommandInfo command, TimeSpan length)
        {
            UserId = userId;
            GuildId = guildId;
            Command = command;
            EndsAt = DateTimeOffset.UtcNow.Add(length);
        }

        public ulong UserId { get; }
        public ulong GuildId { get; }
        public CommandInfo Command { get; }
        public DateTimeOffset EndsAt { get; set; }
    }
}
