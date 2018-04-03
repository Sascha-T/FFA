using Discord.Commands;
using FFA.Extensions.Discord;
using System;
using System.Threading.Tasks;

namespace FFA.Preconditions.Parameter
{
    public sealed class MinHoursAttribute : ParameterPreconditionAttribute
    {
        private readonly double _minimumHours;

        public MinHoursAttribute(double minimumHours)
        {
            _minimumHours = minimumHours;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter,
            object value, IServiceProvider services)
        {
            if (value is TimeSpan length && length.TotalHours < _minimumHours)
                return Task.FromResult(
                    PreconditionResult.FromError($"The minimum {parameter.Format()} is {_minimumHours}h."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
