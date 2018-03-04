using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Preconditions.Command
{
    public sealed class AttachedImage : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var attachment = context.Message.Attachments.FirstOrDefault();

            if (attachment == default(Attachment))
                return Task.FromResult(PreconditionResult.FromError("You must attach an image."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
