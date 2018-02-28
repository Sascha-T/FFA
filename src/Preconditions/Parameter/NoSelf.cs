using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace FFA.Preconditions.Parameter
{
    public sealed class NoSelfAttribute : ParameterPreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services)
        {
            if (value is IUser user && user.Id == context.User.Id)
                return Task.FromResult(PreconditionResult.FromError("This command may not be used on yourself."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
