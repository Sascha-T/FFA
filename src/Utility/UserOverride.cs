using Discord.Commands;
using FFA.Readers;

namespace FFA.Utility
{
    // TODO: remove when D.NET fixes spaget
    public sealed class UserOverride : OverrideTypeReaderAttribute
    {
        public UserOverride() : base(typeof(UserTypeReader))
        {
        }
    }
}
