using Discord;
using Discord.Commands;

namespace FFA.Extensions.Discord
{
    public static class CommandInfoExtensions
    {
        public static string GetUsage(this CommandInfo command)
        {
            var usage = command.Name;

            foreach (var param in command.Parameters)
                usage += $" {(param.IsOptional ? "[" : "<")}{param.Format()}{(param.IsOptional ? "]" : ">")}";

            return usage;
        }

        public static string GetExample(this CommandInfo command)
        {
            var example = command.Name;

            foreach (var param in command.Parameters)
            {
                var before = string.Empty;

                if (typeof(IUser).IsAssignableFrom(param.Type) || typeof(IRole).IsAssignableFrom(param.Type))
                    before += "@";
                else if (typeof(ITextChannel).IsAssignableFrom(param.Type))
                    before += "#";

                example += $" {before}{param.Summary ?? param.Format()}";
            }

            return example;
        }
    }
}
