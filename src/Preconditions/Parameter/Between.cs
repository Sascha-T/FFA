using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace FFA.Preconditions.Parameter
{
    public class BetweenAttribute : ParameterPreconditionAttribute
    {
        private readonly int _minimum;
        private readonly int _maximum;

        public BetweenAttribute(int minimum, int maximum)
        {
            _minimum = minimum;
            _maximum = maximum;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services)
        {
            if (value is int number && (number < _minimum || number > _maximum))
                return Task.FromResult(PreconditionResult.FromError($"The {parameter.Name} must be between {_minimum} and {_maximum}."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
