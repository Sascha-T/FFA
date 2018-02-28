using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace FFA.Preconditions.Parameter
{
    // TODO: no minimum hours? only generic minimum attribute?
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
            // TODO: proper way to parse parameter name's and split the spaces.
            if (value is TimeSpan length && length.TotalHours < _minimumHours)
                return Task.FromResult(PreconditionResult.FromError($"The minimum {parameter.Name} is {_minimumHours}h."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
