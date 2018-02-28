using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace FFA.Preconditions.Parameter
{
    public sealed class MinimumHoursAttribute : ParameterPreconditionAttribute
    {
        private readonly uint _minimumHours;

        public MinimumHoursAttribute(uint minimumHours)
        {
            _minimumHours = minimumHours;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value,
            IServiceProvider services)
        {
            if (value is uint hours && hours < _minimumHours)
                return Task.FromResult(PreconditionResult.FromError($"The minimum {parameter.Name} is {_minimumHours}h."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
