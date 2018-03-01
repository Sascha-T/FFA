using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Database.Models;
using FFA.Extensions.Database;
using FFA.Extensions.Discord;
using FFA.Preconditions.Command;
using FFA.Preconditions.Parameter;
using FFA.Services;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Modules
{
    [NotMuted]
    public sealed class General : ModuleBase<Context>
    {
        private readonly IMongoCollection<CustomCmd> _customCmdCollection;
        private readonly CustomCmdService _customCmdService;

        public General(IMongoDatabase db, CustomCmdService customCmdService)
        {
            _customCmdCollection = db.GetCollection<CustomCmd>("commands");
            _customCmdService = customCmdService;
        }

        [Command("Color")]
        [Alias("colour")]
        [Summary("Give yourself a role with any color you please.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Top(Config.TOP_COLOR)]
        public async Task ColorAsync([Summary("#FF0000")] [Remainder] [Cooldown(Config.COLOR_CD)] Color color)
        {
            var role = await Context.Guild.GetOrCreateRoleAsync(color.GetFormattedString(), color);
            var existingColorRoles = Context.GuildUser.GetRoles().Where(x => x.Name.StartsWith('#'));

            await Context.GuildUser.RemoveRolesAsync(existingColorRoles);
            await Context.GuildUser.AddRoleAsync(role);
            await Context.ReplyAsync("You have successfully set your role color.");
        }

        [Command("AddCommand")]
        [Alias("addcmd")]
        [Summary("Add any custom command you please.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task AddCommandAsync([Summary("retarded")] [UniqueCustomCmdAttribute] string name,
            [Summary("vim2meta LMAO, dude is thick as balls")] [Remainder] [MaximumLength(Config.MAX_CMD_LENGTH)] string response)
        {
            var sterilized = _customCmdService.SterilizeResponse(response);

            // TODO: move to type reader to avoid repeating code!!!
            if (sterilized.Length == 0)
            {
                await Context.ReplyErrorAsync("You have provided an invalid command response.");
            }
            else
            {
                var newCommand = new CustomCmd(Context.User.Id, Context.Guild.Id, name.ToLower(), sterilized);

                await _customCmdCollection.InsertOneAsync(newCommand);
                await Context.ReplyAsync("You have successfully created a new custom command.");
            }
        }

        [Command("ModifyCommand")]
        [Top(Config.TOP_MOD_COMMAND)]
        [Alias("modcommand", "modcmd")]
        [Summary("Modify an existing custom command.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task ModifyCommandAsync([Summary("retarded")] CustomCmd command,
            [Summary("vim2meta LMAO, dude is thick as balls")] [Remainder] [Cooldown(Config.MOD_CMD_CD)]
            [MaximumLength(Config.MAX_CMD_LENGTH)]  string response)
        {
            var sterilized = _customCmdService.SterilizeResponse(response);

            if (sterilized.Length == 0)
            {
                await Context.ReplyErrorAsync("You have provided an invalid command response.");
            }
            else
            {
                await _customCmdCollection.UpdateAsync(command, x => x.Response = response);
                await Context.ReplyAsync("You have successfully updated this command.");
            }
        }
    }
}
