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
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Modules
{
    [Name("General")]
    [Summary("The best memes in town start with these commands.")]
    [NotMuted]
    public sealed class General : ModuleBase<Context>
    {
        private readonly IMongoCollection<CustomCmd> _dbCustomCmds;
        private readonly CustomCmdService _customCmdService;
        private readonly ColorRoleService _colorRoleService;
        private readonly CooldownService _cooldownService;

        public General(IMongoCollection<CustomCmd> dbCustomCmds, CustomCmdService customCmdService, ColorRoleService colorRoleService,
            CooldownService cooldownService)
        {
            _dbCustomCmds = dbCustomCmds;
            _customCmdService = customCmdService;
            _colorRoleService = colorRoleService;
            _cooldownService = cooldownService;
        }

        [Command("Color")]
        [Alias("colour")]
        [Summary("Give yourself a role with any color you please.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Top(Config.TOP_COLOR)]
        [Cooldown(Config.COLOR_CD)]
        public async Task ColorAsync([Summary("#FF0000")] [Remainder] Color color)
        {
            var role = await _colorRoleService.GetOrCreateAsync(Context.Guild, _colorRoleService.FormatColor(color), color);
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
            [Summary("VIM2META LOL DUDE IS THICC AS BALLS")] [Remainder] [MaximumLength(Config.MAX_CMD_LENGTH)] CmdResponse response)
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
        [Cooldown(Config.MOD_CMD_CD)]
        public async Task ModifyCommandAsync([Summary("vim2meta")] CustomCmd command,
            [Summary("RETARD THAT'S AS BLIND AS ME GRAN")] [Remainder] [MaximumLength(Config.MAX_CMD_LENGTH)] CmdResponse response = null)
        {
            await _dbCustomCmds.UpdateAsync(command, x => x.Response = response.Value);
            await Context.ReplyAsync("You have successfully updated this command.");
        }

        [Command("Cooldowns")]
        [Alias("cd", "cooldown", "cds")]
        [Summary("View anyone's command cooldowns.")]
        public async Task CooldownsAsync([Summary("jimbo#8237")] [Remainder] IUser user = null)
        {
            user = user ?? Context.User;
            var cooldowns = _cooldownService.GetAllCooldowns(user.Id, Context.Guild.Id);

            if (cooldowns.Count() == 0)
            {
                _ = user.Id == Context.User.Id ?
                    await Context.ReplyAsync("All your commands are available for use.") :
                    await Context.SendAsync($"All of {user.Bold()}'s commands are available for use.");
            }
            else
            {
                var description = string.Empty;

                foreach (var cd in cooldowns)
                    description += $"**{cd.Command.Name}:** {cd.EndsAt.Subtract(DateTimeOffset.UtcNow).ToString(@"hh\:mm\:ss")}\n";

                await Context.SendAsync(description, $"{user}'s Cooldowns");
            }
        }
    }
}
