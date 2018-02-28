using FFA.Common;
using FFA.Database.Models;
using FFA.Extensions.Database;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class CustomCommandService
    {
        private readonly IMongoCollection<CustomCommand> _customCommandCollection;

        public CustomCommandService(IMongoCollection<CustomCommand> customCommandCollection)
        {
            _customCommandCollection = customCommandCollection;
        }

        public async Task ExecuteAsync(Context context, int argPos)
        {
            var cmdName = context.Message.Content.Substring(argPos).Split(' ').FirstOrDefault().ToLower();
            var customCmd = await _customCommandCollection.FindOneAsync(x => x.GuildId == context.Guild.Id && x.Name == cmdName);

            if (customCmd != default(CustomCommand))
            {
                // TODO: check for perms or try catch?
                // TODO: let sender throw BUT add a perms check in messge recieved
                await context.Channel.SendMessageAsync(customCmd.Response);
            }
        }
    }
}
