using Discord;
using Discord.Commands;

namespace FFA.Extensions
{
    public static class CommandInfoExtensions
    {
        public static string GetUsage(this CommandInfo command)
        {
            var usage = command.Name;

            foreach (var param in command.Parameters)
            {
                var before = param.IsOptional ? "<" : "[";
                var after = param.IsOptional ? ">" : "]";

                if (param.Type.IsAssignableFrom(typeof(IUser)) || param.Type.IsAssignableFrom(typeof(IRole)))
                {
                    before += "@";
                }
                else if (param.Type.IsAssignableFrom(typeof(ITextChannel)))
                {
                    before += "#";
                }

                usage += $" {before}{param.Name}{after}";
            }

            return usage;
        }

        public static string GetExample(this CommandInfo command)
        {
            var example = command.Name;

            foreach (var param in command.Parameters)
            {
                var before = string.Empty;

                if (param.Type.IsAssignableFrom(typeof(IUser)) || param.Type.IsAssignableFrom(typeof(IRole)))
                {
                    before += "@";
                }
                else if (param.Type.IsAssignableFrom(typeof(ITextChannel)))
                {
                    before += "#";
                }

                example += $" {before}{param.Summary ?? param.Name}";
            }

            return example;
        }
    }
}
