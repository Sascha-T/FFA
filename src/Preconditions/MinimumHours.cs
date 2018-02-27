using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace FFA.Preconditions
{
    public sealed class MinimumHours : ParameterPreconditionAttribute
    {
        private readonly uint _minimumHours;

        public MinimumHours(uint minimumHours)
        {
            _minimumHours = minimumHours;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services)
        {
            // TODO: variables to hold casted value
            if ((uint)value < _minimumHours)
                return Task.FromResult(PreconditionResult.FromError($"The minimum {parameter.Name} is {_minimumHours}h."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
