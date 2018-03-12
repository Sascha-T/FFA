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
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FFA.Modules
{
    [Name("General")]
    [Summary("The best memes in town start with these commands.")]
    [NotMuted]
    public sealed class General : ModuleBase<Context>
    {
        private readonly IMongoCollection<CustomCmd> _dbCustomCmds;
        private readonly IMongoCollection<Mute> _dbMutes;
        private readonly CustomCmdService _customCmdService;
        private readonly ColorRoleService _colorRoleService;

        public General(IMongoCollection<CustomCmd> dbCustomCmds, IMongoCollection<Mute> dbMutes, CustomCmdService customCmdService, ColorRoleService colorRoleService)
        {
            _dbCustomCmds = dbCustomCmds;
            _dbMutes = dbMutes;
            _customCmdService = customCmdService;
            _colorRoleService = colorRoleService;
        }

        [Command("Color")]
        [Alias("colour")]
        [Summary("Give yourself a role with any color you please.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Top(Config.TOP_COLOR)]
        [Cooldown(Config.COLOR_CD)]
        public async Task ColorAsync(
            [Summary("#FF0000")] [Remainder] Color color)
        {
            var role = await _colorRoleService.GetOrCreateAsync(Context.Guild, _colorRoleService.FormatColor(color), color);
            var existingColorRoles = Context.GuildUser.GetRoles().Where(x => x.Name.StartsWith('#'));

            await Context.GuildUser.RemoveRolesAsync(existingColorRoles);
            await Context.GuildUser.AddRoleAsync(role);
            await Context.ReplyAsync("You have successfully set your role color.");
        }

        [Command("AddEmote")]
        [Alias("addemoji", "createemote", "createemoji")]
        [Summary("Add an emote.")]
        [AttachedImage]
        [Cooldown(Config.ADD_EMOTE_CD)]
        [Top(Config.TOP_ADD_EMOTE)]
        [AvailableEmoteSlots]
        public async Task AddEmoteAsync(
            [Summary("nice")] string name)
        {
            var attachment = Context.Message.Attachments.First();
            var stream = new MemoryStream(new WebClient().DownloadData(new Uri(attachment.Url)));
            await Context.Guild.CreateEmoteAsync(name, new Image(stream));
            await Context.ReplyAsync("You have successfully added a new emote.");
        }

        [Command("RemoveEmote")]
        [Alias("removeemoji", "deleteemote", "deleteemoji")]
        [Summary("Remove an emote.")]
        [Top(Config.TOP_REMOVE_EMOTE)]
        [Cooldown(Config.REMOVE_EMOTE_CD)]
        public async Task RemoveEmoteAsync(
            [Summary(":nice:")] GuildEmote emote)
        {
            await Context.Guild.DeleteEmoteAsync(emote);
            await Context.ReplyAsync("You have successfully removed this emote.");
        }

        [Command("AddCommand")]
        [Alias("addcmd", "createcommand", "createcmd")]
        [Summary("Add any custom command you please.")]
        public async Task AddCommandAsync(
            [Summary("retarded")] [UniqueCustomCmd] string name,
            [Summary("VIM2META LOL DUDE IS THICC AS BALLS")] [Remainder] [MaximumLength(Config.MAX_CMD_LENGTH)] CmdResponse response)
        {
            var newCmd = new CustomCmd(Context.User.Id, Context.Guild.Id, name.ToLower(), response.Value);
            await _dbCustomCmds.InsertOneAsync(newCmd);
            await Context.ReplyAsync("You have successfully added a new custom command.");
        }

        [Command("ModifyCommand")]
        [Alias("modcommand", "modcmd", "modifycmd")]
        [Summary("Modify an existing custom command.")]
        [Top(Config.TOP_MOD_CMD)]
        [Cooldown(Config.MOD_CMD_CD)]
        public async Task ModifyCommandAsync(
            [Summary("vim2meta")] CustomCmd command,
            [Summary("RETARD THAT'S AS BLIND AS ME GRAN")] [Remainder] [MaximumLength(Config.MAX_CMD_LENGTH)] CmdResponse response = null)
        {
            await _dbCustomCmds.UpdateAsync(command, x => x.Response = response.Value);
            await Context.ReplyAsync("You have successfully updated this command.");
        }

        [Command("RemoveCommand")]
        [Alias("removecmd", "deletecommand", "deletecmd")]
        [Summary("Delete an existing custom command.")]
        [Top(Config.TOP_REMOVE_CMD)]
        [Cooldown(Config.REMOVE_CMD_CD)]
        public async Task RemoveCommandAsync(
            [Summary("vim2meta")] CustomCmd command)
        {
            await _dbCustomCmds.DeleteOneAsync(command);
            await Context.ReplyAsync("You have successfully deleted this command.");
        }

        [Command("TimeLeft")]
        [Alias("left")]
        [Summary("Tell how much time is left on your mute.")]
        public async Task TimeLeftAsync(
            [Summary("hornydevil#0018")] [Remainder] IUser user = null)
        {
            user = user ?? Context.User;

            var dbMuteUser = await _dbMutes.FindOneAsync(x => x.UserId == user.Id && x.GuildId == Context.Guild.Id && x.Active);

            if (dbMuteUser == null)
                await Context.ReplyErrorAsync($"{(user != Context.User ? user + " isn\'t" : "You aren\'t")} muted.");
            else
            {
                var timeLeft = dbMuteUser.Timestamp.Add(dbMuteUser.Length).Subtract(DateTimeOffset.UtcNow);
                await Context.SendAsync($"**Time left:** {timeLeft.ToString(@"hh\:mm\:ss")}", $"{user}'s Mute");
            }
        }
    }
}
