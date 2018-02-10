using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace FFA.Preconditions
{
    public sealed class NoSelf : ParameterPreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services)
        {
            if (context.User.Id == ((IUser)value).Id)
            {
                return Task.FromResult(PreconditionResult.FromError("This command may not be used on yourself."));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
