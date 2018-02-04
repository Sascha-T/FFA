using Discord;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace FFA.Common
{
    public static class Configuration
    {
        // Command handler
        public const string Prefix = ";";

        // Current user
        public const string Game = Prefix + "help";
        
        // Moderation
        public const int TopReputation = 20, MinimumMuteLength = 1;

        // Cooldowns
        public const int RepCooldown = 24, UnRepCooldown = 24;

        // Custom colors
        public static readonly Color ErrorColor = new Color(0xFF0000), MuteColor = new Color(0xFF3E29), UnmuteColor = new Color(0xFF3E29);

        // Default colors
        public static readonly IReadOnlyList<Color> Colors = new Color[]
        {
            new Color(0xFF269A),
            new Color(0x00FF00),
            new Color(0x00E828),
            new Color(0x08F8FF),
            new Color(0xF226FF),
            new Color(0xFF1C8E),
            new Color(0x68FF22),
            new Color(0xFFBE11),
            new Color(0x2954FF),
            new Color(0x9624ED),
            new Color(0xA8ED00)
        }.ToImmutableArray();
    }
}
