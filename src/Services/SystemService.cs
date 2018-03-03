using Discord.Commands;
using FFA.Common;
using FFA.Entities.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FFA.Services
{
    public sealed class ListService : Service
    {
        public string ListCommands(IEnumerable<CommandInfo> cmds)
            => List(cmds, x => x.Name, x => x.Name.Length, x => x.Summary);

        public string ListModules(IEnumerable<ModuleInfo> modules)
            => List(modules, x => x.Name, x => x.Name.Length, x => x.Summary);

        public string List<T, TSummary>(IEnumerable<T> elements, Func<T, string> nameSelector, Func<T, int> lengthSelector,
            Func<T, TSummary> summarySelector)
        {
            var list = "```";
            var padding = nameSelector(elements.OrderByDescending(lengthSelector).First()).Length + 2;

            foreach (var element in elements.OrderBy(nameSelector))
                list += $"{Config.PREFIX}{nameSelector(element).PadRight(padding)}{summarySelector(element)}\n";

            return $"{list}```";
        }
    }
}
