using Discord;
using Newtonsoft.Json;

namespace FFA.Common
{
    public class Configuration
    {
        // TODO: JSON reader for Color type
        [JsonConstructor]
        public Configuration(char prefix, uint[] colors, uint errorColor)
        {
            Prefix = prefix;
            Colors = new Color[colors.Length];
            
            for (int i = 0; i < colors.Length; i++)
            {
                Colors[i] = new Color(colors[i]);
            }

            ErrorColor = new Color(errorColor);
        }

        public char Prefix { get; set; }
        
        public Color[] Colors { get; set; }

        public Color ErrorColor { get; set; }
    }
}
