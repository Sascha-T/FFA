using FFA.Common;
using FFA.Database.Models;
using FFA.Entities.Service;
using FFA.Extensions.Database;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Services
{
    // TODO: organize services into sub folders
    public sealed class CustomCmdService : Service
    {
        private readonly IMongoCollection<CustomCmd> _dbCustomCmds;

        public CustomCmdService(IMongoCollection<CustomCmd> dbCustomCmds)
        {
            _dbCustomCmds = dbCustomCmds;
        }

        public async Task ExecuteAsync(Context context, int argPos)
        {
            var cmdName = context.Message.Content.Substring(argPos).Split(' ').FirstOrDefault();

            if (string.IsNullOrWhiteSpace(cmdName))
                return;

            var customCmd = await _dbCustomCmds.FindCustomCmdAsync(cmdName, context.Guild.Id);

            if (customCmd != null)
                await context.Channel.SendMessageAsync(customCmd.Response);
                await _dbCustomCmds.UpdateAsync(customCmd, x => x.Uses++);
        }

        public string SterilizeResponse(string input)
        {
            input = input.Replace("@", string.Empty);

            if (input.Count(x => x == '\n') > Config.MAX_CMD_NEW_LINES)
                input = Config.NEW_LINE_REGEX.Replace(input, string.Empty);

            return input.Trim();
        }
    }
}
