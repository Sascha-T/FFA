using Discord;

namespace FFA.Extensions
{
    public static class IUserExtensions
    {
        public static string Tag(this IUser user)
        {
            return "**" + user.Username + user.Discriminator + "**";
        }
    }
}
