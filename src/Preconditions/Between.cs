using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace FFA.Preconditions
{
    public class Between : ParameterPreconditionAttribute
    {
        private readonly int _minimum;
        private readonly int _maximum;

        public Between(int minimum, int maximum)
        {
            _minimum = minimum;
            _maximum = maximum;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services)
        {
            var castedValue = (int)value;

            if (castedValue < _minimum || castedValue > _maximum)
            {
                return Task.FromResult(PreconditionResult.FromError($"The {parameter.Name} must be between {_minimum} and {_maximum}."));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
