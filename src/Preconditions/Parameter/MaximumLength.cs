using System;
using System.Threading.Tasks;
using Discord.Commands;
using FFA.Extensions.Discord;

namespace FFA.Preconditions.Parameter
{
    public sealed class MaximumLengthAttribute : ParameterPreconditionAttribute
    {
        private readonly int _length;

        public MaximumLengthAttribute(int length)
        {
            _length = length;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext ctx, ParameterInfo param, object value,
            IServiceProvider services)
        {
            var strValue = $"{value}";

            if (strValue.Length > _length)
                return Task.FromResult(PreconditionResult.FromError($"The maximum {param.Format()} length is {_length} characters."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
