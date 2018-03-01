using FFA.Common;
using FFA.Database.Models;
using FFA.Extensions.Database;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Services
{
    // TODO: organize services into sub folders
    public sealed class CustomCmdService
    {
        private readonly IMongoCollection<CustomCmd> _customCmdCollection;

        public CustomCmdService(IMongoDatabase db)
        {
            _customCmdCollection = db.GetCollection<CustomCmd>("commands");
        }

        public async Task ExecuteAsync(Context context, int argPos)
        {
            var cmdName = context.Message.Content.Substring(argPos).Split(' ').FirstOrDefault().ToLower();
            var customCmd = await _customCmdCollection.FindOneAsync(x => x.GuildId == context.Guild.Id && x.Name == cmdName);

            if (customCmd != null)
            {
                // TODO: check for perms or try catch?
                // TODO: let SendingService throw BUT add a perms check in messge recieved
                await context.Channel.SendMessageAsync(customCmd.Response);
            }
        }

        public string SterilizeResponse(string input)
        {
            input = Config.MENTION_REGEX.Replace(input, string.Empty);

            if (input.Count(x => x == '\n') > Config.MAX_CMD_NEW_LINES)
                input = Config.NEW_LINE_REGEX.Replace(input, string.Empty);

            return input.Trim();
        }
    }
}
