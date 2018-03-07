using Discord;
using Discord.Commands;
using System.Text;

namespace FFA.Extensions.Discord
{
    public static class CommandInfoExtensions
    {
        public static string GetUsage(this CommandInfo command)
        {
            var usageBuilder = new StringBuilder(command.Name);

            foreach (var param in command.Parameters)
                usageBuilder.AppendFormat(" {0}{1}{2}", param.IsOptional ? "[" : "<", param.Format(), param.IsOptional ? "]" : ">");

            return usageBuilder.ToString();
        }

        public static string GetExample(this CommandInfo command)
        {
            var exampleBuilder = new StringBuilder(command.Name);

            foreach (var param in command.Parameters)
            {
                var before = string.Empty;

                if (typeof(IUser).IsAssignableFrom(param.Type) || typeof(IRole).IsAssignableFrom(param.Type))
                    before = "@";
                else if (typeof(ITextChannel).IsAssignableFrom(param.Type))
                    before = "#";

                exampleBuilder.AppendFormat(" {0}{1}", before, param.Summary ?? param.Format());
            }

            return exampleBuilder.ToString();
        }
    }
}
