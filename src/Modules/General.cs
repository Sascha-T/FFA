using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Database.Models;
using FFA.Entities.CustomCmd;
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
        private readonly IMongoCollection<CustomCmd> _dbCustomCmds;
        private readonly CustomCmdService _customCmdService;

        public General(IMongoCollection<CustomCmd> dbCustomCmds, CustomCmdService customCmdService)
        {
            _dbCustomCmds = dbCustomCmds;
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

        // TODO: return custom runtime result instead of using loads of if/else + ReplyErrorAsync!!!
        [Command("AddCommand")]
        [Alias("addcmd")]
        [Summary("Add any custom command you please.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task AddCommandAsync([Summary("retarded")] [UniqueCustomCmdAttribute] string name,
            [Summary("VIM2META LOL DUDE IS THICC AS BALLS")] [Remainder] [MaximumLength(Config.MAX_CMD_LENGTH)] CmdResponse response = null)
        {
            var newCmd = new CustomCmd(Context.User.Id, Context.Guild.Id, name.ToLower(), response.Value);
            await _dbCustomCmds.InsertOneAsync(newCmd);
            await Context.ReplyAsync("You have successfully created a new custom command.");
        }

        [Command("ModifyCommand")]
        [Top(Config.TOP_MOD_COMMAND)]
        [Alias("modcommand", "modcmd", "modifycmd")]
        [Summary("Modify an existing custom command.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task ModifyCommandAsync([Summary("vim2meta")] CustomCmd command,
            [Summary("RETARD THAT'S AS BLIND AS ME GRAN")] [Remainder] [Cooldown(Config.MOD_CMD_CD)]
            [MaximumLength(Config.MAX_CMD_LENGTH)] CmdResponse response = null)
        {
            await _dbCustomCmds.UpdateAsync(command, x => x.Response = response.Value);
            await Context.ReplyAsync("You have successfully updated this command.");
        }
    }
}
