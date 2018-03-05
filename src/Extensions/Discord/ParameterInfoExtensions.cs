using Discord.Commands;
using FFA.Common;

namespace FFA.Extensions.Discord
{
    public static class ParameterInfoExtensions
    {
        public static string Format(this ParameterInfo param)
            => Config.CAMEL_CASE.Replace(param.Name, " $1").ToLower();
    }
}
