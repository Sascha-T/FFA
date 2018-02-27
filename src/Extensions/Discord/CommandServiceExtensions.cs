using Discord.Commands;

namespace FFA.Extensions.Discord
{
    internal static class CommandServiceExtensions
    {
        internal static CommandInfo GetCommand(this CommandService commandService, ICommandContext context, int argPos)
            => commandService.Search(context, argPos).Commands[0].Command;
    }
}
