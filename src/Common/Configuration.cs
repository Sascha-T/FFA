using Discord;
using System.Collections.Immutable;

namespace FFA.Common
{
    public static class Configuration
    {
        public const string Prefix = ";";
        public const string Game = Prefix + "help";
        public static readonly Color ErrorColor = new Color(0xFF0000);
        public static readonly Color MuteColor = new Color(0xFF3E29);
        public static readonly Color UnmuteColor = new Color(0xFF3E29);
        public static readonly ImmutableArray<Color> Colors = new ImmutableArray<Color>()
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
        };
        
    }
}
