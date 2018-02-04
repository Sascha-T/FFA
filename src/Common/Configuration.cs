using Discord;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace FFA.Common
{
    public static class Configuration
    {
        // Command handler
        public const string PREFIX = ";";

        // Current user
        public const string GAME = PREFIX + "help";
        
        // Moderation
        public const int TOP_REP = 20, MIN_MUTE_LENGTH = 1;

        // Cooldowns
        public const int REP_COOLDOWN = 24, UNREP_COOLDOWN = 24;

        // Custom colors
        public static readonly Color ERROR_COLOR = new Color(0xFF0000), MUTE_COLOR = new Color(0xFF3E29), UNMUTE_COLOR = new Color(0x72FF65);

        // Default colors
        // TODO: more colors!!!
        public static readonly IReadOnlyList<Color> DEFAULT_COLORS = new Color[]
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

        // JSON serialization
        public static readonly JsonSerializerSettings JSON_SETTINGS = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error
        };
    }
}
