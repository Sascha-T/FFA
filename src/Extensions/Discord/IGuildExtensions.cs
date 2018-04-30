using Discord;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Extensions.Discord
{
    public static class IGuildExtensions
    {
        public static async Task<ITextChannel> GetGeneralAsync(this IGuild guild)
        {
            var channels = await guild.GetTextChannelsAsync();
            return channels.FirstOrDefault(x => x.Name == "general");
        }
    }
}
