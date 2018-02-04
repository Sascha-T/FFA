﻿using Discord;
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
        private readonly Configuration _config;

        public System(CommandService commandService, Configuration config)
        {
            _commandService = commandService;
            _config = config;
        }

        [Command("Help")]
        [Alias("command", "commands", "cmd", "cmds")]
        [Summary("Information about all the commands.")]
        public async Task Help([Summary("rep")] string commandName = null)
        {
            if (!string.IsNullOrWhiteSpace(commandName))
            {
                var cmd = _commandService.Commands.FirstOrDefault(x 
                            => x.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase) || 
                               x.Aliases.Any(y => y.Equals(commandName, StringComparison.OrdinalIgnoreCase)));

                if (cmd == default(CommandInfo))
                {
                    await Context.ReplyErrorAsync("This command does not exist.");
                }
                else
                {
                    await Context.SendAsync($"**Description:** {cmd.Summary}\n" +
                                            $"**Usage:** `{_config.Prefix}{cmd.GetUsage()}`\n" +
                                            $"**Example:** `{_config.Prefix}{cmd.GetExample()}`");
                }
            }
            else
            {
                var description = "```";
                var padding = _commandService.Commands.OrderByDescending(x => x.Name.Length).First().Name.Length + 2;

                foreach (var command in _commandService.Commands.OrderBy(x => x.Name))
                {
                    description += $"{_config.Prefix}{command.Name.PadRight(padding)}{command.Summary}\n";
                }

                await Context.DmAsync($"{description}```", "Commands");

                if (!(Context.Channel is IDMChannel))
                {
                    await Context.ReplyAsync("You have been DMed with all the commands.");
                }
            }
        }
    }
}