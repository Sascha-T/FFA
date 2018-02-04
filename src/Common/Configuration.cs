using Discord;

namespace FFA.Common
{
    public class Configuration
    {
        // TODO: JSON reader for Color type
        public Configuration(char prefix, uint[] colors, uint errorColor, uint muteColor, uint unmuteColor)
        {
            Prefix = prefix;
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
        public Color[] Colors { get; set; }
        public Color ErrorColor { get; set; }
        public Color MuteColor { get; set; }
        public Color UnmuteColor { get; set; }
    }
}
