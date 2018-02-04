using Discord;

namespace FFA.Common
{
    public class Configuration
    {
        // TODO: JSON reader for Color type
        // TODO: constant static config file?
        public Configuration(char prefix, string game, uint[] colors, uint errorColor, uint muteColor, uint unmuteColor)
        {
            Prefix = prefix;
            Game = game;
            Colors = new Color[colors.Length];
            
            for (var i = 0; i < colors.Length; i++)
            {
                Colors[i] = new Color(colors[i]);
            }

            ErrorColor = new Color(errorColor);
            MuteColor = new Color(muteColor);
            UnmuteColor = new Color(unmuteColor);
        }

        public char Prefix { get; set; }
        public string Game { get; set; }
        public Color[] Colors { get; set; }
        public Color ErrorColor { get; set; }
        public Color MuteColor { get; set; }
        public Color UnmuteColor { get; set; }
    }
}
