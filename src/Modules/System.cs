using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Modules
{
    [Name("System")]
    public sealed class System : ModuleBase<Context>
    {
        private readonly CommandService _commandService;

        public System(CommandService commandService)
        {
            _commandService = commandService;
        }

        [Command("Help")]
        [Alias("command", "commands", "cmd", "cmds")]
        [Summary("Information about all the commands.")]
        public async Task HelpAsync([Summary("rep")] string commandName = null)
        {
            if (!string.IsNullOrWhiteSpace(commandName))
            {
                var cmd = _commandService.Commands.FirstOrDefault(x => x.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase) ||
                                                                       x.Aliases.Any(y => y.Equals(commandName, StringComparison.OrdinalIgnoreCase)));

                if (cmd == default(CommandInfo))
                    await Context.ReplyErrorAsync("This command does not exist.");
                else
                    await Context.SendAsync($"**Description:** {cmd.Summary}\n" +
                                            $"**Usage:** `{Configuration.PREFIX}{cmd.GetUsage()}`\n" +
                                            $"**Example:** `{Configuration.PREFIX}{cmd.GetExample()}`");
            }
            else
            {
                var description = "```";
                var padding = _commandService.Commands.OrderByDescending(x => x.Name.Length).First().Name.Length + 2;

                foreach (var command in _commandService.Commands.OrderBy(x => x.Name))
                    description += $"{Configuration.PREFIX}{command.Name.PadRight(padding)}{command.Summary}\n";

                await Context.DmAsync($"{description}```", "Commands");

                if (!(Context.Channel is IDMChannel))
                    await Context.ReplyAsync("You have been DMed with all the commands.");
            }
        }
    }
}
