using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace FFA.Preconditions
{
    public sealed class MaximumLength : ParameterPreconditionAttribute
    {
        private readonly int _length;
        
        public MaximumLength(int length)
        {
            _length = length;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services)
        {
            if (((string)value).Length > _length)
            {
                return Task.FromResult(PreconditionResult.FromError($"The maximum {parameter.Name} length is {_length} characters."));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
