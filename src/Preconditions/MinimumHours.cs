using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace FFA.Preconditions
{
    public class MinimumHours : ParameterPreconditionAttribute
    {
        private readonly int _minimumHours;

        public MinimumHours(int minimumHours)
        {
            _minimumHours = minimumHours;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services)
        {
            if (((TimeSpan)value).TotalHours < _minimumHours)
            {
                return Task.FromResult(PreconditionResult.FromError($"The minimum {parameter.Name} is {_minimumHours}h."));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
