using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Database.Models;
using FFA.Extensions.Database;
using FFA.Extensions.Discord;
using FFA.Preconditions.Command;
using FFA.Preconditions.Parameter;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Modules
{
    [NotMuted]
    public sealed class General : ModuleBase<Context>
    {
        private readonly IMongoCollection<CustomCommand> _customCommandCollection;

        public General(IMongoCollection<CustomCommand> customCommandCollection)
        {
            _customCommandCollection = customCommandCollection;
        }

        [Command("Color")]
        [Alias("colour")]
        [Summary("Give yourself a role with any color you please.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Top(Configuration.TOP_COLOR)]
        public async Task ColorAsync([Summary("#FF0000")] [Remainder] [Cooldown(Configuration.COLOR_COOLDOWN)] Color color)
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
        public async Task AddCommandAsync([Summary("retarded")] [UniqueCustomCommand] string name,
            [Summary("vim2meta LMAO, dude is thick as balls")] [Remainder] string response)
        {
            await _customCommandCollection.InsertOneAsync(new CustomCommand(Context.User.Id, Context.Guild.Id, name.ToLower(), response));
            await Context.ReplyAsync("You have successfully created a new custom command.");
        }

        [Command("ModifyCommand")]
        [Top(Configuration.TOP_MOD_COMMAND)]
        [Alias("modcommand", "modcmd")]
        [Summary("Modify an existing custom command.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task ModifyCommandAsync([Summary("retarded")] CustomCommand command,
            [Summary("vim2meta LMAO, dude is thick as balls")] [Remainder] [Cooldown(Configuration.MOD_COMMAND_COOLDOWN)] string response)
        {
            await _customCommandCollection.UpdateAsync(command, x => x.Response = response);
            await Context.ReplyAsync("You have successfully updated this command.");
        }
    }
}
