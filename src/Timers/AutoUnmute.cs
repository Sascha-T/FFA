using Discord.WebSocket;
using FFA.Common;
using FFA.Database;
using FFA.Database.Models;
using System;
using System.Threading;

namespace FFA.Timers
{
    public class AutoUnmute
    {
        private readonly DiscordSocketClient _client;
        private readonly Credentials _credentials;
        private readonly FFAContext _ffaContext;
        private readonly Timer _timer;
        private readonly AutoResetEvent _autoEvent;

        public AutoUnmute(DiscordSocketClient client, Credentials credentials, FFAContext ffaContext)
        {
            _credentials = credentials;
            _client = client;
            _ffaContext = ffaContext;
            _autoEvent = new AutoResetEvent(false);
            // TODO: put auto unnut length in configuration
            _timer = new Timer(Execute, _autoEvent, 0, 60000);
        }

        private async void Execute(object state)
        {
            var guild = _client.GetGuild(_credentials.GuildId);
            var mutedRole = guild.GetRole(_credentials.MutedRoleId);

            // TODO: safe to remove during iteration?
            foreach (var mute in _ffaContext.Mutes)
            {
                if (mute.EndsAt.Subtract(DateTime.UtcNow).Ticks <= 0)
                {
                    await _ffaContext.RemoveAsync<Mute>(mute.Id);

                    var guildUser = guild.GetUser(mute.UserId);

                    if (guildUser != null)
                    {
                        await guildUser.RemoveRoleAsync(mutedRole);
                    }
                }
            }
        }
    }
}
