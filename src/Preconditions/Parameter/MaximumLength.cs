using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace FFA.Preconditions.Parameter
{
    public sealed class MaximumLengthAttribute : ParameterPreconditionAttribute
    {
        private readonly int _length;

        public MaximumLengthAttribute(int length)
        {
            _length = length;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services)
        {
            if (value is string strValue && strValue.Length > _length)
                return Task.FromResult(PreconditionResult.FromError($"The maximum {parameter.Name} length is {_length} characters."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
