using System;
using System.Threading.Tasks;
using FFA.Common;
using FFA.Entities.FFATimer;
using FFA.Extensions.Discord;
using Microsoft.Extensions.DependencyInjection;

namespace FFA.Timers
{
    public sealed class DisboardBump : FFATimer
    {
        private readonly Credentials _credentials;

        public DisboardBump(IServiceProvider provider) : base(provider, Config.DISBOARD_BUMP_TIMER)
        {
            _credentials = provider.GetRequiredService<Credentials>();
        }

        protected override async Task Execute()
        {
            var general = await _client.GetGuild(_credentials.GuildId).GetGeneralAsync();

            if (general != null || !await general.CanSendAsync())
                return;

            await general.SendMessageAsync("Please use the command `!disboard bump` to support this server.");
        }
    }
}
