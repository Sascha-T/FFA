using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Extensions.Discord;
using FFA.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Modules
{
    // TODO: make less memes of the command examples!
    [Name("System")]
    [Summary("System commands directly tied with FFA's functionality.")]
    public sealed class System : ModuleBase<Context>
    {
        private readonly CommandService _commandService;
        private readonly SystemService _systemService;

        public System(CommandService commandService, SystemService systemService)
        {
            _commandService = commandService;
            _systemService = systemService;
        }

        [Command("Help")]
        [Alias("information", "info")]
        [Summary("Information about the bot.")]
        public async Task Help()
        {
            await Context.DmAsync(Config.HELP_MESSAGE, "FFA Information");

            if (!(Context.Channel is IDMChannel))
                await Context.ReplyAsync("You have been DMed with bot information.");
        }

        [Command("Modules")]
        [Alias("category", "categories")]
        [Summary("List of all the command modules.")]
        public Task ModulesAsync()
            => Context.SendAsync(_systemService.ListModules(_commandService.Modules), "Modules");

        [Command("Commands")]
        [Alias("cmds", "cmdlist", "commandlist")]
        [Summary("List of all the commands.")]
        public async Task CommandsAsync()
        {
            await Context.DmAsync(_systemService.ListCommands(_commandService.Commands), "Commands");

            if (!(Context.Channel is IDMChannel))
                await Context.ReplyAsync("You have been DMed with a list of all commands.");
        }

        [Command("Module")]
        [Alias("modulelist")]
        [Summary("List of all commands of a specific module.")]
        public Task ModuleAsync(string name)
        {
            var module = _commandService.Modules.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (module == default(ModuleInfo))
                return Context.ReplyErrorAsync("This module does not exist.");
            else
                return Context.SendAsync(_systemService.ListCommands(module.Commands), $"{module.Name}'s Commands");
        }

        [Command("Command")]
        [Alias("commandinfo", "cmd", "cmdinfo")]
        [Summary("Information about a specific command.")]
        public async Task HelpAsync([Summary("rep")] string name)
        {
            var cmd = _commandService.Commands.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                    x.Aliases.Any(y => y.Equals(name, StringComparison.OrdinalIgnoreCase)));

            if (cmd == default(CommandInfo))
                await Context.ReplyErrorAsync("This command does not exist.");
            else
                await Context.SendAsync($"**Description:** {cmd.Summary}\n" +
                    $"**Usage:** `{Config.PREFIX}{cmd.GetUsage()}`\n" +
                    $"**Example:** `{Config.PREFIX}{cmd.GetExample()}`", cmd.Name);
        }
    }
}
